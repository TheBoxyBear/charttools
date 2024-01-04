using ChartTools.IO.Formatting;
using ChartTools.IO.Configuration;
using ChartTools.IO.Midi.Parsing;
using ChartTools.IO.Midi.Mapping;
using ChartTools.IO.Midi.Configuration;
using ChartTools.IO.Midi.Configuration.Sessions;

namespace ChartTools.IO.Midi;

public static class MidiFile
{
    /// <summary>
    /// Default configuration to use for reading when the provided configuration is <see langword="default"/>
    /// </summary>
    public static MidiReadingConfiguration DefaultReadConfig { get; set; } = new()
    {
        InvalidMidiEventPolicy         = InvalidMidiEventPolicy.ThrowException,
        DuplicateTrackObjectPolicy     = DuplicateTrackObjectPolicy.ThrowException,
        MisalignedBigRockMarkersPolicy = MisalignedBigRockMarkersPolicy.ThrowException,
        MissingBigRockMarkerPolicy     = MissingBigRockMarkerPolicy.ThrowException,
        OverlappingSpecialPhrasePolicy = OverlappingSpecialPhrasePolicy.ThrowException,
        SnappedNotesPolicy             = SnappedNotesPolicy.Snap,
        SoloNoStarPowerPolicy          = SoloNoStarPowerPolicy.Convert,
        UnopenedTrackObjectPolicy      = UnopenedTrackObjectPolicy.ThrowException,
        UnclosedTrackObjectPolicy      = UnclosedTrackObjectPolicy.ThrowException,
        UncertainFormatPolicy          = UncertainFormatPolicy.ThrowException,
        UnknownSectionPolicy           = UnknownSectionPolicy.ThrowException
    };

    public static MidiWritingConfiguration DefaultWriteConfig { get; set; } = new()
    {
        DuplicateTrackObjectPolicy     = DuplicateTrackObjectPolicy.ThrowException,
        OverlappingSpecialPhrasePolicy = OverlappingSpecialPhrasePolicy.ThrowException,
        SnappedNotesPolicy             = SnappedNotesPolicy.Snap,
        SoloNoStarPowerPolicy          = SoloNoStarPowerPolicy.Convert,
        UncertainFormatPolicy          = UncertainFormatPolicy.ThrowException,
        UnsupportedModifierPolicy      = UnsupportedModifierPolicy.ThrowException
    };

    /// <summary>
    /// Creates a <see cref="MidiParser"/> for parsing a section based on the header.
    /// </summary>
    /// <exception cref="FormatException"></exception>
    private static MidiParser? GetSongParser(string header, MidiReadingSession session, ref byte index)
    {
        index++;

        return header switch
        {
            MidiFormatting.GlobalEventHeader => new GlobalEventParser(session),
            MidiFormatting.GHGemsHeader      => new StandardInstrumentParser(new GHGemsMapper(session), StandardInstrumentIdentity.LeadGuitar, session),
            MidiFormatting.LeadGuitarHeader  => new StandardInstrumentParser(new GuitarBassMapper(session), StandardInstrumentIdentity.LeadGuitar, session),
            MidiFormatting.BassHeader        => new StandardInstrumentParser(new GuitarBassMapper(session), StandardInstrumentIdentity.Bass, session),
            MidiFormatting.AnimHeader        => new AnimParser(session),
            _ => index == 1 ? new TitleSyncTrackParser(header, session) : null,
        };
    }

    /// <summary>
    /// Combines the results from the parsers of a <see cref="MidiFileReader"/> into a <see cref="Song"/>.
    /// </summary>
    /// <param name="reader">Reader to get the parsers from</param>
    private static Song CreateSongFromReader(MidiFileReader reader)
    {
        Song song = new();

        foreach (var parser in reader.Parsers)
            parser.ApplyToSong(song);

        song.Metadata.Formatting.Resolution = reader.Resolution;

        return song;
    }

    /// <inheritdoc cref="Song.FromFile(string, ReadingConfiguration?, FormattingRules?)"/>
    /// <param name="path"><inheritdoc cref="Song.FromFile(string, ReadingConfiguration?, FormattingRules?)" path="/param[@name='path']"/></param>
    /// <param name="config"><inheritdoc cref="Song.FromFile(string, ReadingConfiguration?, FormattingRules?)" path="/param[@name='config']"/></param>
    public static Song ReadSong(string path, MidiReadingConfiguration? config = default, FormattingRules? formatting = default)
    {
        byte count = 0;
        var reader = new MidiFileReader(path, header => GetSongParser(header, new(config, formatting ?? new()), ref count), config?.FirstPassReadingSettings);

        reader.Read();
        return CreateSongFromReader(reader);
    }
    /// <inheritdoc cref="Song.FromFileAsync(string, ReadingConfiguration?, FormattingRules?, CancellationToken)"/>
    /// <param name="path"><inheritdoc cref="Song.FromFileAsync(string, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@='path']"/></param>
    /// <param name="cancellationToken"><inheritdoc cref="Song.FromFileAsync(string, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@='cancellationToken']"/></param>
    /// <param name="config"><inheritdoc cref="Song.FromFileAsync(string, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@='config']"/></param>
    public static async Task<Song> ReadSongAsync(string path, MidiReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
    {
        byte count = 0;
        var session = new MidiReadingSession(config, formatting ?? new());
        var reader = new MidiFileReader(path, header => GetSongParser(header, session, ref count), config?.FirstPassReadingSettings);

        await reader.ReadAsync(cancellationToken);
        return CreateSongFromReader(reader);
    }
}
