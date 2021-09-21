using ChartTools.Collections.Unique;
using ChartTools.InternalTools;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Ini;

using System;
using System.Linq;

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
        public string? Title { get; set; }
        /// <summary>
        /// Artist or band behind the <see cref="Song"/>
        /// </summary>
        public string? Artist { get; set; }
        /// <summary>
        /// Album featuring the <see cref="Song"/>
        /// </summary>
        public string? Album { get; set; }
        /// <summary>
        /// Track number of the song within the album
        /// </summary>
        public ushort? AlbumTrack { get; set; }
        /// <summary>
        /// Track number of the song within the playlist/setlist
        /// </summary>
        public ushort? PlaylistTrack { get; set; }
        /// <summary>
        /// Year of release
        /// </summary>
        public ushort? Year { get; set; }
        /// <summary>
        /// Genre of the <see cref="Song"/>
        /// </summary>
        public string? Genre { get; set; }
        /// <summary>
        /// Creator of the chart
        /// </summary>
        public Charter Charter { get; set; } = new Charter();
        /// <summary>
        /// Start time in milliseconds of the preview in the Clone Hero song browser
        /// </summary>
        public uint? PreviewStart { get; set; }
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
        public sbyte? Difficulty { get; set; }
        /// <summary>
        /// Type of media the audio track comes from
        /// </summary>
        public string? MediaType { get; set; }
        /// <summary>
        /// Number of <see cref="TrackObject.Position"/> values per beat
        /// </summary>
        public ushort Resolution { get; set; }
        /// <summary>
        /// Offset of the audio track in milliseconds. A higher value makes the audio start sooner.
        /// </summary>
        public int? AudioOffset { get; set; }
        /// <summary>
        /// Paths of audio files
        /// </summary>
        public StreamCollection Streams { get; set; } = new StreamCollection();
        /// <summary>
        /// Offset of the background video in milliseconds. A higher value makes the video start sooner.
        /// </summary>
        public int? VideoOffset { get; set; }
        /// <summary>
        /// Length of the song in milliseconds
        /// </summary>
        public uint? Length { get; set; }
        /// <summary>
        /// Text to be displayed on the load screen
        /// </summary>
        public string? LoadingText { get; set; }
        /// <summary>
        /// The song is a modchart
        /// </summary>
        public bool IsModchart { get; set; }
        public UniqueList<MetadataItem> UnidentifiedData { get; } = new((a, b) => a.Key == b.Key);
        #endregion

        /// <summary>
        /// Reads the metadata from a file.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        public static Metadata FromFile(string path) => ExtensionHandler.Read(path, (".chart", ChartParser.ReadMetadata), (".ini", IniParser.ReadMetadata));
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
        /// <inheritdoc cref="IniParser.WriteMetadata(string, Metadata)"/>
        public void ToFile(string path) => ExtensionHandler.Write(path, this, (".ini", IniParser.WriteMetadata));
    }

    /// <summary>
    /// Creator of the chart
    /// </summary>
    public class Charter
    {
        /// <summary>
        /// Name of the creator
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Location of the image file to use as an icon in the Clone Hero song browser
        /// </summary>
        public string? Icon { get; set; }
    }

    /// <summary>
    /// Set of audio files to play and mute during gameplay
    /// </summary>
    /// <remarks>Instrument audio may be muted when chords of the respective instrument are missed</remarks>
    public class StreamCollection
    {
        /// <summary>
        /// Location of the base audio file
        /// </summary>
        public string? Music { get; set; }
        /// <summary>
        /// Location of the guitar audio file
        /// </summary>
        public string? Guitar { get; set; }
        /// <summary>
        /// Location of the bass audio
        /// </summary>
        public string? Bass { get; set; }
        /// <summary>
        /// Location of the rhythm guitar audio file
        /// </summary>
        public string? Rhythm { get; set; }
        /// <summary>
        /// Location of the keys audio file
        /// </summary>
        public string? Keys { get; set; }
        /// <summary>
        /// Location of the drums' kicks audio file
        /// </summary>
        /// <remarks>Can include all drums audio</remarks>
        public string? Drum { get; set; }
        /// <summary>
        /// Location of the drums' snares audio file
        /// </summary>
        /// <remarks>Can include all drums audio except kicks</remarks>
        public string? Drum2 { get; set; }
        /// <summary>
        /// Location of the drum's toms audio file
        /// </summary>
        /// <remarks>Can include toms and cymbals</remarks>
        public string? Drum3 { get; set; }
        /// <summary>
        /// Location of the drum's cymbals audio file
        /// </summary>
        public string? Drum4 { get; set; }
        /// <summary>
        /// Location of the vocals audio file
        /// </summary>
        public string? Vocal { get; set; }
        /// <summary>
        /// Location of the crowd reaction audio file
        /// </summary>
        public string? Crowd { get; set; }
    }

    public struct MetadataItem
    {
        public string Key { get; init; }
        public string Data { get; set; }
        public FileFormat Origin { get; set; }
    }
}
