using ChartTools.Formatting;
using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Parsing;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools.IO.Midi
{
    public static class MidiFile
    {
        /// <summary>
        /// Default configuration to use for reading when the provided configuration is <see langword="default"/>
        /// </summary>
        public static ReadingConfiguration DefaultReadConfig { get; set; } = new()
        {
            DuplicateTrackObjectPolicy = DuplicateTrackObjectPolicy.ThrowException,
            SoloNoStarPowerPolicy = SoloNoStarPowerPolicy.Convert,
            MidiFirstPassReadingSettings = null
        };

        /// <summary>
        /// Creates a <see cref="MidiParser"/> for parsing a section based on the header.
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private static MidiParser GetSongParser(string header, ReadingSession session)
        {
            switch (header)
            {
                case MidiFormatting.GHGemsHeader:
                    return new GHGemsParser(session);
                default:
                    return null!;
            }
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

            return song;
        }

        /// <inheritdoc cref="Song.FromFile(string, ReadingConfiguration?, FormattingRules?)"/>
        /// <param name="path"><inheritdoc cref="Song.FromFile(string, ReadingConfiguration?, FormattingRules?)" path="/param[@name='path']"/></param>
        /// <param name="config"><inheritdoc cref="Song.FromFile(string, ReadingConfiguration?, FormattingRules?)" path="/param[@name='config']"/></param>
        public static Song ReadSong(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default)
        {
            config ??= DefaultReadConfig;

            var reader = new MidiFileReader(path, header => GetSongParser(header, new(config, formatting ?? new())), config.MidiFirstPassReadingSettings);

            reader.Read();
            return CreateSongFromReader(reader);
        }
        /// <inheritdoc cref="Song.FromFileAsync(string, ReadingConfiguration?, FormattingRules?, CancellationToken)"/>
        /// <param name="path"><inheritdoc cref="Song.FromFileAsync(string, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@='path']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="Song.FromFileAsync(string, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="Song.FromFileAsync(string, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@='config']"/></param>
        public static async Task<Song> ReadSongAsync(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        {
            config ??= DefaultReadConfig;

            var reader = new MidiFileReader(path, header => GetSongParser(header, new(config, formatting ?? new())), config.MidiFirstPassReadingSettings);

            await reader.ReadAsync(cancellationToken);
            return CreateSongFromReader(reader);
        }
    }
}
