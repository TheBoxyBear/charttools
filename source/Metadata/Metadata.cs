using ChartTools.Formatting;
using ChartTools.Internal;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Ini;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        [IniSimpleSerialize(IniFormatting.Title)]
        public string? Title { get; set; }

        /// <summary>
        /// Artist or band behind the <see cref="Song"/>
        /// </summary>
        [IniSimpleSerialize(IniFormatting.Artist)]
        public string? Artist { get; set; }

        [IniSimpleSerialize(IniFormatting.Album)]
        /// <summary>
        /// Album featuring the <see cref="Song"/>
        /// </summary>
        public string? Album { get; set; }

        /// <summary>
        /// Track number of the song within the album
        /// </summary>
        public ushort? AlbumTrack { get; set; }

        /// <summary>
        /// Playlist that the song should show up in
        /// </summary>
        [IniSimpleSerialize(IniFormatting.Playlist)]
        public string? Playlist { get; set; }

        /// <summary>
        /// Sub-playlist that the song should show up in
        /// </summary>
        [IniSimpleSerialize(IniFormatting.SubPlaylist)]
        public string? SubPlaylist { get; set; }

        /// <summary>
        /// Track number of the song within the playlist/setlist
        /// </summary>
        [IniSimpleSerialize(IniFormatting.PlaylistTrack)]
        public ushort? PlaylistTrack { get; set; }

        /// <summary>
        /// Year of release
        /// </summary>
        [IniSimpleSerialize(IniFormatting.Year)]
        public ushort? Year { get; set; }

        /// <summary>
        /// Genre of the <see cref="Song"/>
        /// </summary>
        [IniSimpleSerialize(IniFormatting.Genre)]
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
        [IniSimpleSerialize(IniFormatting.PreviewStart)]
        public uint? PreviewStart { get; set; }

        [IniSimpleSerialize(IniFormatting.PreviewEnd)]
        /// <summary>
        /// End time in milliseconds of the preview in the Clone Hero song browser
        /// </summary>
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
        [IniSimpleSerialize(IniFormatting.Difficulty)]
        public sbyte? Difficulty { get; set; }

        /// <summary>
        /// Type of media the audio track comes from
        /// </summary>
        public string? MediaType { get; set; }

        /// <summary>
        /// Offset of the audio track in milliseconds. A higher value makes the audio start sooner.
        /// </summary>
        [IniSimpleSerialize(IniFormatting.AudioOffset)]
        public int? AudioOffset { get; set; }

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
        /// Offset of the background video in milliseconds. A higher value makes the video start sooner.
        /// </summary>
        /// <summary>
        /// Offset of the background video in milliseconds. A higher value makes the video start sooner.
        /// </summary>
        public int? VideoOffset { get; set; }

        /// <summary>
        /// Length of the song in milliseconds
        /// </summary>
        [IniSimpleSerialize(IniFormatting.Length)]
        public uint? Length { get; set; }

        /// <summary>
        /// Text to be displayed on the load screen
        /// </summary>
        [IniSimpleSerialize(IniFormatting.LoadingText)]
        public string? LoadingText { get; set; }

        [IniSimpleSerialize(IniFormatting.Modchart)]
        /// <summary>
        /// The song is a modchart
        /// </summary>
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

        /// <summary>
        /// Reads the metadata from a file.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        public static Metadata FromFile(string path) => ExtensionHandler.Read<Metadata>(path, null, (".chart", (path, _) => ChartFile.ReadMetadata(path)), (".ini", (path, _) => IniParser_old.ReadMetadata(path)));
        public static async Task<Metadata> FromFileAsync(string path, CancellationToken cancellationToken) => await ExtensionHandler.ReadAsync<Metadata>(path, cancellationToken, null, (".chart", (path, token, _) => ChartFile.ReadMetadataAsync(path, token)), (".ini", (path, _, _) => Task.Run(() => IniParser_old.ReadMetadata(path))));
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
        public static Metadata FromFiles(params string[] paths)
        {
            // No files provided
            if (paths is null || paths.Length == 0)
                throw new ArgumentException("No provided paths");

            Metadata[] data = new Metadata[paths.Length];

            // Read all files
            for (int i = 0; i < paths.Length; i++)
                data[i] = FromFile(paths[i]);

            data[0].Merge(false, data.Skip(1).ToArray());

            return data[0];
        }
        /// <inheritdoc cref="IniParser_old.WriteMetadata(string, Metadata)"/>
        public void ToFile(string path) => ExtensionHandler.Write(path, (".ini", path => IniParser_old.WriteMetadata(path, this)));
    }
}
