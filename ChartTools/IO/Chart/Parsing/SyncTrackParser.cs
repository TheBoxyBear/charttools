using ChartTools.Extensions.Linq;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

using System.Runtime.InteropServices;

namespace ChartTools.IO.Chart.Parsing;

internal class SyncTrackParser(ChartReadingSession session)
	: ChartParser(session, ChartFormatting.SyncTrackHeader.AsMemory())
{
	public override SyncTrack Result
		=> GetResult(result);

	private readonly SyncTrack result = new();

	private readonly List<Tempo> tempos = [], orderedTempos = [];
	private readonly List<Anchor> orderedAnchors = [];
	private readonly List<TimeSignature> orderedSignatures = [];

	protected override void HandleItem(in ReadOnlyMemory<char> line)
	{
		TrackObjectEntry entry = new(line);
		ReadOnlySpan<char> data = entry.Data.Span;

		switch (entry.Type.Span)
		{
			case "TS": // Time signature
				if (CheckDuplicate(orderedSignatures, "time signature", out int newIndex))
					break;

				ReadOnlySpan<char> a, b;
				ChartFormatting.SplitData(data, out a, out b);

				byte
					numerator = ValueParser.Parse<byte>(in a, "numerator"),
					denominator = 4;

				// Denominator is only written if not equal to 4
				if (!b.IsEmpty)
					denominator = (byte)Math.Pow(2, ValueParser.Parse<byte>(in b, "denominator"));

				TimeSignature signature = new(entry.Position, numerator, denominator);

				result.TimeSignatures.Add(signature);
				orderedSignatures.Insert(newIndex, signature);
				break;
			case "B": // Tempo
				if (CheckDuplicate(orderedTempos, "tempo marker", out newIndex))
					break;

				// Floats are written by rounding to the 3rd decimal and removing the decimal point
				float value = ValueParser.Parse<float>(data, "value") / 1000;
				Tempo tempo = new(entry.Position, value);

				tempos.Add(tempo);
				orderedTempos.Add(tempo);
				break;
			case "A": // Anchor
				if (CheckDuplicate(orderedAnchors, "tempo anchor", out newIndex))
					break;

				// Floats are written by rounding to the 3rd decimal and removing the decimal point
				TimeSpan anchor = TimeSpan.FromSeconds(ValueParser.Parse<float>(data, "anchor") / 1000);

				orderedAnchors.Insert(newIndex, new(entry.Position, anchor));
				break;
		}

		bool CheckDuplicate<T>(IList<T> existing, string objectType, out int newIndex)
			where T : IReadOnlyTrackObject
		{
			int index = 0;
			bool result = !Session.HandleDuplicate(entry.Position, objectType, () =>
			{
				index = existing.BinarySearchIndex<T, uint>(entry.Position, static t => t.Position, out bool exactMatch);

				return exactMatch;
			});

			newIndex = index;

			return result;
		}
	}

	protected override void FinalizeParse()
	{
		foreach (ref readonly Anchor anchor in CollectionsMarshal.AsSpan(orderedAnchors))
		{
			// Find the marker matching the position in case it was already added through a mention of value
			int markerIndex = orderedTempos.BinarySearchIndex(anchor.Position, static t => t.Position, out bool markerFound);

			if (markerFound)
			{
				orderedTempos[markerIndex].Anchor = anchor.Value;
				orderedTempos.RemoveAt(markerIndex);
			}
			else if (Session.HandleTempolessAnchor(anchor))
				result.Tempo.Add(new(anchor.Position, 0) { Anchor = anchor.Value });
		}

		base.FinalizeParse();
	}

	public override void ApplyToSong(Song song)
	{
		song.SyncTrack = Result;
		song.SyncTrack.Tempo.AddRange(tempos);
	}
}
