using ChartTools.Extensions;
using ChartTools.IO.Formatting;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Serialization;
using ChartTools.IO.Ini;

using System;
using System.Collections.Generic;

namespace ChartTools
{
    /// <summary>
    /// Set of miscellaneous information about a <see cref="Song"/>
    /// </summary>
    public class Metadata
    {
        #region Properties
        /// <summary>
        /// Title of the <see cref="Song"/>
        /// </summary>
        [ChartKeySerializable(ChartFormatting.Title)]
        [IniKeySerializable(IniFormatting.Title)]
        public string? Title { get; set; }

        /// <summary>
        /// Artist or band behind the <see cref="Song"/>
        /// </summary>
        [ChartKeySerializable(ChartFormatting.Artist)]
        [IniKeySerializable(IniFormatting.Artist)]
        public string? Artist { get; set; }

        /// <summary>
        /// Album featuring the <see cref="Song"/>
        /// </summary>
        [ChartKeySerializable(ChartFormatting.Album)]
        [IniKeySerializable(IniFormatting.Album)]
        public string? Album { get; set; }

        /// <summary>
        /// Track number of the song within the album
        /// </summary>
        public ushort? AlbumTrack { get; set; }

        /// <summary>
        /// Playlist that the song should show up in
        /// </summary>
        [IniKeySerializable(IniFormatting.Playlist)]
        public string? Playlist { get; set; }

        /// <summary>
        /// Sub-playlist that the song should show up in
        /// </summary>
        [IniKeySerializable(IniFormatting.SubPlaylist)]
        public string? SubPlaylist { get; set; }

        /// <summary>
        /// Track number of the song within the playlist/setlist
        /// </summary>
        [IniKeySerializable(IniFormatting.PlaylistTrack)]
        public ushort? PlaylistTrack { get; set; }

        /// <summary>
        /// Year of release
        /// </summary>
        [IniKeySerializable(IniFormatting.Year)]
        public ushort? Year { get; set; }

        /// <summary>
        /// Genre of the <see cref="Song"/>
        /// </summary>
        [ChartKeySerializable(ChartFormatting.Genre)]
        [IniKeySerializable(IniFormatting.Genre)]
        public string? Genre { get; set; }

        /// <summary>
        /// Creator of the chart
        /// </summary>
        public Charter Charter
        {
            get => _charter;
            set => _charter = value ?? throw new ArgumentNullException(nameof(value));
        }
        private Charter _charter = new();

        /// <summary>
        /// Start time in milliseconds of the preview in the Clone Hero song browser
        /// </summary>
        [ChartKeySerializable(ChartFormatting.PreviewStart)]
        [IniKeySerializable(IniFormatting.PreviewStart)]
        public uint? PreviewStart { get; set; }

        /// <summary>
        /// End time in milliseconds of the preview in the Clone Hero song browser
        /// </summary>
        [ChartKeySerializable(ChartFormatting.PreviewEnd)]
        [IniKeySerializable(IniFormatting.PreviewEnd)]
        public uint? PreviewEnd { get; set; }

        /// <summary>
        /// Duration in milliseconds of the preview in the Clone Hero song browser
        /// </summary>
        public uint PreviewLength
        {
            get
            {
                if (PreviewEnd is null)
                    return 30000;

                return PreviewStart is null ? PreviewEnd.Value : PreviewEnd.Value - PreviewStart.Value;
            }
        }

        /// <summary>
        /// Overall difficulty of the song
        /// </summary>
        [ChartKeySerializable(ChartFormatting.Difficulty)]
        [IniKeySerializable(IniFormatting.Difficulty)]
        public sbyte? Difficulty { get; set; }
        /// <inheritdoc cref="InstrumentDifficultySet"/>
        public InstrumentDifficultySet InstrumentDifficulties
        {
            get => _instrumentDifficulties;
            set => _instrumentDifficulties = value ?? throw new ArgumentNullException(nameof(value));
        }
        private InstrumentDifficultySet _instrumentDifficulties = new();
        /// <summary>
        /// Type of media the audio track comes from
        /// </summary>
        [ChartKeySerializable(ChartFormatting.MediaType)]
        public string? MediaType { get; set; }

        /// <summary>
        /// Offset of the audio track. A higher value makes the audio start sooner.
        /// </summary>
        [ChartKeySerializable(ChartFormatting.AudioOffset)]
        [IniKeySerializable(IniFormatting.AudioOffset)]
        public TimeSpan? AudioOffset { get; set; }

        /// <summary>
        /// Paths of audio files
        /// </summary>
        public StreamCollection Streams
        {
            get => _streams;
            set => _streams = value ?? throw new ArgumentNullException(nameof(value));
        }
        private StreamCollection _streams = new();

        /// <summary>
        /// Offset of the background video. A higher value makes the video start sooner.
        /// </summary>
        public TimeSpan? VideoOffset { get; set; }

        /// <summary>
        /// Length of the song in milliseconds
        /// </summary>
        [IniKeySerializable(IniFormatting.Length)]
        public uint? Length { get; set; }

        /// <summary>
        /// Text to be displayed on the load screen
        /// </summary>
        [IniKeySerializable(IniFormatting.LoadingText)]
        public string? LoadingText { get; set; }

        /// <summary>
        /// The song is a modchart
        /// </summary>
        [IniKeySerializable(IniFormatting.Modchart)]
        public bool IsModchart { get; set; }


        private FormattingRules _formatting = new();
        /// <inheritdoc cref="FormattingRules"/>
        public FormattingRules Formatting
        {
            get => _formatting;
            set => _formatting = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Unrecognized metadata
        /// </summary>
        /// <remarks>When writing, these will only be written if the target format matches the origin</remarks>
        public HashSet<UnidentifiedMetadata> UnidentifiedData { get; } = new(new FuncEqualityComparer<UnidentifiedMetadata>((a, b) => a.Key == b.Key));
        #endregion

        public void ReadFile(string path) => Read(path, this);
        /// <summary>
        /// Reads the metadata from a file.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="LineException"/>
        /// <exception cref="OutOfMemoryException"/>
        public static Metadata FromFile(string path) => Read(path);

        private static Metadata Read(string path, Metadata? existing = null) => ExtensionHandler.Read<Metadata>(path, (".chart", ChartFile.ReadMetadata), (".ini", path => IniFile.ReadMetadata(path, existing)));

        /// <summary>
        /// Reads the metadata from multiple files.
        /// </summary>
        /// <remarks>Each file has less priority than the preceding.</remarks>
        /// <param name="paths">Paths of the files to read</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        public static Metadata? FromFiles(params string[] paths)
        {
            // No files provided
            if (paths is null || paths.Length == 0)
                throw new ArgumentException("No provided paths");

            var data = FromFile(paths[0]);

            foreach (var path in paths[1..])
                data.ReadFile(path);

            return data;
        }

        public void ToFile(string path) => ExtensionHandler.Write<Metadata>(path, this, (".ini", IniFile.WriteMetadata));
    }
}
