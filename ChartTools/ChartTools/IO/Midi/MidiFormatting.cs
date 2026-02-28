using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi;

internal static class MidiFormatting
{
	private static readonly Dictionary<InstrumentIdentity, string> InstrumentSequenceNames = new()
	{
		{ InstrumentIdentity.StandardLeadGuitar, LeadGuitarHeader },
		{ InstrumentIdentity.StandardCoopGuitar, CoopGuitarHeader },
		{ InstrumentIdentity.StandardBass, BassHeader },
		{ InstrumentIdentity.StandardRhythmGuitar, RhythmGuitarHeader },
		{ InstrumentIdentity.StandardKeys, KeysHeader },
		{ InstrumentIdentity.Drums, DrumsHeader },
		{ InstrumentIdentity.GHLLeadGuitar, GHLGuitarHeader },
		{ InstrumentIdentity.GHLBass, GHLBassHeader },
		{ InstrumentIdentity.Vocals, VocalsHeader }
	};

	public const string
		BassHeader         = "PART BASS",
		CoopGuitarHeader   = "PART GUITAR COOP",
		DrumsHeader        = "PART DRUMS",
		GHGemsHeader       = "T1 GEMS",
		GlobalEventHeader  = "EVENTS",
		GHLGuitarHeader    = "PART GUITAR GHL",
		GHLBassHeader      = "PART BASS GHL",
		KeysHeader         = "PART KEYS",
		LeadGuitarHeader   = "PART GUITAR",
		RhythmGuitarHeader = "PART RHYTHM",
		VocalsHeader       = "PART VOCALS",
		AnimHeader         = "ANIM";

	public static string Instrument(InstrumentIdentity instrument) => InstrumentSequenceNames[instrument];

	public static IEnumerable<string> PotentialHeaders(InstrumentIdentity identity)
	{
		switch (identity)
		{
			case InstrumentIdentity.StandardLeadGuitar:
				yield return GHGemsHeader;
				yield return LeadGuitarHeader;
				break;
			case InstrumentIdentity.StandardBass:
				yield return BassHeader;
				break;
		}
	}

	public static bool FindChunk(IEnumerable<TrackChunk> chunks, Predicate<string> match, [NotNullWhen(true)] out string? header, [NotNullWhen(true)] out IEnumerator<MidiEvent>? enumerator)
	{
		foreach (EventsCollection events in chunks.Select(c => c.Events))
		{
			using IEnumerator<MidiEvent> eventsEnumerator = events.GetEnumerator();

			if (eventsEnumerator.MoveNext())
				continue;
			if (eventsEnumerator.Current is not SequenceTrackNameEvent headerEvent)
				continue;
			if (match(headerEvent.Text))
				continue;

			header = headerEvent.Text;
			enumerator = eventsEnumerator;
			return true;
		}

		header = null;
		enumerator = null;
		return false;
	}
}
