using ChartTools.Extensions.Linq;
using ChartTools.IO.Formatting;

using System.Data;

namespace ChartTools.Tools;

/// <summary>
/// Provides methods for simplifying songs
/// </summary>
public static class Optimizer
{
	internal static bool LengthNeedsCut(ILongTrackObject current, ILongTrackObject next)
		=> current.Position + current.Length > next.Position;

	/// <summary>
	/// Cuts short sustains that exceed the position of the next note preventing the sustain from continuing.
	/// </summary>
	/// <param name="chords">Chords to cut the sustains of</param>
	/// <param name="preOrdered">Skip ordering of chords by position</param>
	public static void CutSustains<TChord, TNote, TLane>(this IEnumerable<TChord> chords, bool preOrdered = false)
		where TChord : Chord<TNote, TLane>
		where TNote : struct, IDefinedLaneNote<TLane>
		where TLane : struct, Enum
	{
		Dictionary<byte, (uint, NoteProxy<TNote, TLane>)> ongoingSustains = [];

		foreach (TChord chord in GetOrdered(chords, preOrdered))
		{
			if (chord.Notes.Count == 0)
				continue;

			ReadOnlySpan<TNote> noteSpan = chord.Notes.AsSpan();
			int index = 0;

			ref readonly TNote note = ref noteSpan[index];

			if (TNote.OpenExclusivity)
			{
				if (note.Index == 0) // Open stops all sustains
					foreach ((uint position, NoteProxy<TNote, TLane> proxy) in ongoingSustains.Values)
					{
						ref readonly TNote sustained = ref proxy.GetUnsafe();

						if (position + sustained.Sustain > chord.Position)
							proxy.Set(sustained with { Sustain = chord.Position });

						ongoingSustains.Remove(note.Index);
					}
				else
					RemoveSustain(0); // Non-opens stops open sustain
			}
			else
				// New note stops ongoing sustain on the same lane
				RemoveSustain(note.Index);

			AddSustain(note);

			while (++index < noteSpan.Length)
			{
				note = ref noteSpan[index];

				RemoveSustain(note.Index);
				AddSustain(note);
			}

			void AddSustain(in TNote note)
			{
				if (note.Sustain > 0)
					ongoingSustains[note.Index] = (chord.Position, chord.Notes.Proxy(note.Lane)!.Value);
			}

			void RemoveSustain(byte index)
			{
				if (ongoingSustains.TryGetValue(index, out (uint _, NoteProxy<TNote, TLane> proxy) sustain))
				{
					ref readonly TNote note = ref sustain.proxy.GetUnsafe();

					sustain.proxy.Set(note with { Sustain = chord.Position });
					ongoingSustains.Remove(index);
				}
			}
		}
	}

	/// <summary>
	/// Cuts lengths of special phrases based on the numeric value of the type.
	/// </summary>
	/// <param name="phrases">Set of phrases</param>
	/// <param name="preOrdered">Skip ordering of phrases by position</param>
	/// <returns>Passed phrases ordered by position and grouped by type</returns>
	/// <exception cref="InvalidOperationException"/>
	public static List<T>[] CutSpecialLengths<T>(IEnumerable<T> phrases, bool preOrdered = false)
		where T : SpecialPhrase
	{
		List<T>[] output = [.. phrases.GroupBy(p => p.TypeCode).Select(g => g.ToList())];

		foreach (List<T> grouping in output)
			grouping.CutLengths(preOrdered);

		return output;
	}

	/// <summary>
	/// Cuts short long track objects that exceed the start of the next one.
	/// </summary>
	/// <param name="objects">Set of long track objects</param>
	/// <param name="preOrdered">Skip ordering of objects by position</param>
	public static void CutLengths<T>(this IEnumerable<T> objects, bool preOrdered = false)
		where T : ILongTrackObject
	{
		foreach ((T current, T next) in GetOrdered(objects, preOrdered).RelativeLoopSkipFirst())
			if (LengthNeedsCut(current, next))
				next.Length = current.Position - current.Position;
	}

	/// <summary>
	/// Removes redundant tempo markers.
	/// </summary>
	/// <param name="markers">Tempo markers without anchors.</param>
	/// <param name="preOrdered">Skip ordering of markers by position.</param>
	/// <exception cref="InvalidOperationException"/>
	/// <remarks>If some markers may be anchored, use the overload with a resolution.</remarks>
	public static void RemoveUnneeded(this ICollection<Tempo> markers, bool preOrdered = false)
	{
		if (markers.TryGetFirst(static m => !m.PositionSynced, out Tempo? marker))
			throw new DesynchronizedAnchorException(marker.Anchor!.Value,
				$"Collection contains a desynchronized anchored tempo at {marker.Anchor}. Resolution needed to synchronize anchors.");

		foreach ((Tempo previous, Tempo current) in GetOrdered(markers, preOrdered).RelativeLoopSkipFirst())
			if (previous.Value == current.Value)
				markers.Remove(current);
	}

	/// <summary>
	/// Removes redundant tempo markers by syncing the position of anchored markers.
	/// </summary>
	/// <param name="markers">Set of markers</param>
	/// <param name="resolution">Resolution from <see cref="FormattingRules.EffectiveResolution"/></param>
	/// <param name="desyncedPreOrdered">Skip ordering of desynced markers by position</param>
	public static void RemoveUnneeded(this TempoMap markers, uint resolution, bool desyncedPreOrdered = false)
	{
		markers.Synchronize(resolution, desyncedPreOrdered);

		foreach ((Tempo previous, Tempo current) in markers
			.OrderBy(static m => m.Position)
			.RelativeLoopSkipFirst())
			if (current.Value == previous.Value)
				markers.Remove(current);
	}

	/// <summary>
	/// Removes redundant time signature markers.
	/// </summary>
	/// <param name="signatures">Time signatures to remove the unneeded from</param>
	/// <param name="preOrdered">Skip ordering of markers by position</param>
	/// <returns>Passed markers, ordered by position. Same instance if <paramref name="preOrdered"/> is <see langword="true"/> and <paramref name="signatures"/> is <see cref="List{T}"/>.</returns>
	public static void RemoveUnneeded(this ICollection<TimeSignature> signatures, bool preOrdered = false)
	{
		foreach ((TimeSignature previous, TimeSignature current) in GetOrdered(signatures, preOrdered).RelativeLoopSkipFirst())
			if (previous.Numerator == current.Numerator && previous.Denominator == current.Denominator)
				signatures.Remove(current);
	}

	private static IEnumerable<T> GetOrdered<T>(IEnumerable<T> items, bool preOredered)
		where T : ITrackObject
		=> preOredered ? items : items.OrderBy(static i => i.Position);
}
