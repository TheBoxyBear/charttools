using ChartTools.Events;

namespace ChartTools.Tools;

public static class TempoRescaler
{
	/// <summary>
	/// Rescales the length a long object.
	/// </summary>
	/// <param name="obj">Object to rescale</param>
	/// <param name="scale">Positive number where 1 is the current scale.</param>
	public static void Rescale(this ILongObject obj, float scale)
		=> obj.Length = (uint)(obj.Length * scale);

	/// <summary>
	/// Rescales the position of a track object
	/// </summary>
	/// <param name="trackObject">Object to rescale</param>
	/// <param name="scale">Positive number where 1 is the current scale.</param>
	public static void Rescale(this ITrackObject trackObject, float scale)
		=> trackObject.Position = (uint)(trackObject.Position * scale);

	/// <summary>
	/// Rescales the position and length of a long track object
	/// </summary>
	/// <param name="trackObject">Object to rescale</param>
	/// <param name="scale">Positive number where 1 is the current scale.</param>
	public static void Rescale(this ILongTrackObject trackObject, float scale)
	{
		trackObject.Position = (uint)(trackObject.Position * scale);
		trackObject.Length = (uint)(trackObject.Length * scale);
	}

	/// <summary>
	/// Rescales the position and value of a tempo marker.
	/// </summary>
	/// <param name="tempo">Marker to rescale</param>
	/// <param name="scale">Positive number where 1 is the current scale.</param>
	public static void Rescale(this Tempo tempo, float scale)
	{
		tempo.Position = (uint)(tempo.Position * scale);
		tempo.Value *= scale;
	}

	/// <summary>
	/// Rescales the position of a chord and sustain of its notes.
	/// </summary>
	/// <param name="chord">Chord to rescale</param>
	/// <param name="scale">Positive number where 1 is the current scale.</param>
	public static void Rescale(this Chord chord, float scale)
	{
		chord.Position = (uint)(chord.Position * scale);

		foreach (NoteProxy proxy in chord.Notes.ProxyAll())
			proxy.Rescale(scale);
	}

	/// <summary>
	/// Rescales the sustain value of the note represented by the specified proxy.
	/// </summary>
	/// <param name="proxy">Proxy representing the note</param>
	/// <param name="scale">Positive number where 1 is the current scale.</param>
	public static void Rescale(this in NoteProxy proxy, float scale)
	{
		ILaneNote? note = proxy.Get();

		if (note is null)
			return;

		proxy.AddOrSet((uint)(note.Sustain * scale));
	}

	/// <summary>
	/// Rescales the chords in a track.
	/// </summary>
	/// <param name="track">Source of chords</param>
	/// <param name="scale">Positive number where 1 is the current scale.</param>
	public static void Rescale(this Track track, float scale)
	{
		foreach (Chord chord in track.Chords)
			Rescale(chord, scale);

		if (track.LocalEvents is not null)
			foreach (LocalEvent e in track.LocalEvents)
				e.Rescale(scale);
	}

	/// <summary>
	/// Rescales all tracks in an instrument.
	/// </summary>
	/// <param name="instrument">Source of the tracks</param>
	/// <param name="scale">Positive number where 1 is the current scale.</param>
	public static void Rescale(this Instrument instrument, float scale)
	{
		foreach (Track track in instrument.GetExistingTracks())
			track.Rescale(scale);
	}

	/// <summary>
	/// Rescales the tempo and time signatures in a song.
	/// </summary>
	/// <param name="syncTrack">Source of markers</param>
	/// <param name="scale">Positive number where 1 is the current scale.</param>
	public static void Rescale(this SyncTrack syncTrack, float scale)
	{
		foreach (Tempo tempo in syncTrack.Tempo)
			tempo.Rescale(scale);

		foreach (TimeSignature signature in syncTrack.TimeSignatures)
			signature.Rescale(scale);
	}

	/// <summary>
	/// Rescales all instruments, tempo and time signatures.
	/// </summary>
	/// <param name="song">Source of objects</param>
	/// <param name="scale">Positive number where 1 is the current scale.</param>
	public static void Rescale(this Song song, float scale)
	{
		foreach (Instrument instrument in song.Instruments.Existing())
			instrument.Rescale(scale);

		song.SyncTrack?.Rescale(scale);

		if (song.GlobalEvents is not null)
			foreach (GlobalEvent e in song.GlobalEvents)
				e.Rescale(scale);
	}
}
