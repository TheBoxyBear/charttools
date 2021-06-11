using ChartTools.SystemExtensions.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChartTools.IO.Ini
{
    /// <summary>
    /// Provides methods for reading and writing ini files
    /// </summary>
    internal static class IniParser
    {
        /// <summary>
        /// Keys for <see cref="Instrument"/> difficulties
        /// </summary>
        private static readonly Dictionary<string, Instruments> difficultyKeys = new()
        {
            { "diff_drums", Instruments.Drums },
            { "diff_guitarghl", Instruments.GHLGuitar },
            { "diff_bassghl", Instruments.GHLBass },
            { "diff_guitar", Instruments.LeadGuitar },
            { "diff_bass", Instruments.Bass },
            { "diff_keys", Instruments.Keys }
        };
        private static readonly Dictionary<string, string> metadataKeys = new()
        {
            { "Title", "name" },
            { "Artist", "artist" },
            { "Genre", "genre" },
            { "Year", "year" },
            { "PreviewStart", "preview_start_time" },
            { "PreviewEnd", "preview_end_time" },
            { "AudioOffset", "delay" },
            { "VideoOffset", "video_start_time" },
        };

        /// <summary>
        /// Reads metadata from a ini file.
        /// </summary>
        /// <returns>Instance of <see cref="Metadata"/> containing the data in the file</returns>
        /// <param name="path">Path of the file to read</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="System.Security.SecurityException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        internal static Metadata ReadMetadata(string path)
        {
            Metadata metadata = new();
            int intValue;
            uint uintValue;

            foreach (string line in File.ReadLines(path))
            {
                (string header, string value) = GetEntry(line);

                switch (header)
                {
                    case "name":
                        metadata.Title = value;
                        break;
                    case "artist":
                        metadata.Artist = value;
                        break;
                    case "album":
                        metadata.Album = value;
                        break;
                    case "year":
                        metadata.Year = ushort.TryParse(value, out ushort ushortValue) ? ushortValue
                            : throw new FormatException($"Cannot parse year \"{value}\" to {metadata.Year.GetType()}.");
                        break;
                    case "genre":
                        metadata.Genre = value;
                        break;
                    case "charter":
                        metadata.Charter ??= new Charter();
                        metadata.Charter.Name = value;
                        break;
                    case "icon":
                        metadata.Charter ??= new();
                        metadata.Charter.Icon = value;
                        break;
                    case "preview_start_time":
                        metadata.PreviewStart = uint.TryParse(value, out uintValue) ? uintValue
                            : throw new FormatException($"Cannot parse preview start \"{value}\" to {metadata.PreviewStart.GetType()}.");
                        break;
                    case "preview_end_time":
                        metadata.PreviewEnd = uint.TryParse(value, out uintValue) ? uintValue
                            : throw new FormatException($"Cannot parse preview end \"{value}\" to {metadata.PreviewEnd.GetType()}.");
                        break;
                    case "delay":
                        metadata.AudioOffset = int.TryParse(value, out intValue) ? intValue
                            : throw new FormatException($"Cannot parse audio offset \"{value}\" to {metadata.AudioOffset.GetType()}.");
                        break;
                    case "video_start_time":
                        metadata.VideoOffset = int.TryParse(value, out intValue) ? intValue
                            : throw new FormatException($"Cannot parse video offset \"{value}\" to {metadata.VideoOffset.GetType()}.");
                        break;
                    case "loading_text":
                        metadata.LoadingText = value;
                        break;
                }
            }

            return metadata;
        }
        /// <summary>
        /// Writes metadata to a ini file.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        /// <param name="metadata">Metadata to write</param>
        internal static void WriteMetadata(string path, Metadata metadata) => File.WriteAllLines(path, GetLines(metadata).Concat(new List<string>(File.ReadLines(path).Where(l => !metadataKeys.ContainsValue(GetEntry(l).header)))));

        private static IEnumerable<string> GetLines(Metadata metadata)
        {
            yield return "[Song]";

            if (metadata is null)
                yield break;

            Type metadataType = typeof(Metadata);

            // Get the value of all properties whose name is in the dictionary and pair with its matching key, filtered to non-null properties
            foreach ((string key, object value) in metadataKeys.Keys.Select(p => (metadataKeys[p], metadataType.GetProperty(metadataKeys[p]).GetValue(metadata))).Where(t => t.Item2 is not null))
                yield return $"{key} = {value}";

            if (metadata.Charter is not null)
            {
                if (metadata.Charter.Name is not null)
                    yield return $"charter = {metadata.Charter.Name}";
                if (metadata.Charter.Icon is not null)
                    yield return $"icon = {metadata.Charter.Icon}";
            }
        }

        /// <summary>
        /// Reads an <see cref="Instrument"/> difficulty from a ini file.
        /// </summary>
        /// <returns>Difficulty read
        ///     <para><see langword="null"/> if the file does not mention a difficulty for the provided instrument</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <param name="instrument">Instrument to read</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        internal static sbyte? ReadDifficulty(string path, Instruments instrument)
        {
            if (!difficultyKeys.ContainsValue(instrument))
                throw new ArgumentException("Ini files do not support difficulty for this instrument.");

            (string header, string value) entry = default;
            string key = difficultyKeys.Keys.First(k => difficultyKeys[k] == instrument);

            foreach (string line in File.ReadLines(path))
            {
                entry = GetEntry(line);

                if (entry.header == key)
                    break;
            }

            if (entry == default)
                return null;

            return sbyte.TryParse(entry.value, out sbyte difficulty)
                ? difficulty
                : throw new FormatException($"Cannot parse difficulty \"{entry.value}\"");
        }
        /// <summary>
        /// Reads <see cref="Instrument"/> difficulties from a ini file and assigns them to the instruments in a <see cref="Song"/>.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        /// <param name="song">Song to assign the difficulties to</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="FormatException"/>
        internal static void ReadDifficulties(string path, Song song)
        {
            foreach (string line in File.ReadAllLines(path))
            {
                (string header, string value) = GetEntry(line);

                if (difficultyKeys.ContainsKey(header))
                {
                    Instrument inst = song.GetInstrument(difficultyKeys[header]);

                    if (inst is not null)
                        inst.Difficulty = sbyte.TryParse(value, out sbyte difficulty) ? difficulty
                            : throw new FormatException($"Cannot parse difficulty \"{value}\"");
                }
            }
        }
        private static (string header, string value) GetEntry(string line)
        {
            string[] split = line.Split('=', 2);

            split[0] = split[0].Trim().ToLower();
            split[1] = split[1].Trim();

            return (split[0].Trim().ToLower(), split[1].Trim());
        }

        /// <summary>
        /// Writes an <see cref="Instrument"/> difficulty to a ini file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="instrument">Instrument to write the difficulty of</param>
        /// <param name="value">Difficulty to write</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="FormatException"/>
        internal static void WriteDifficulty(string path, Instruments instrument, sbyte value)
        {
            if (!difficultyKeys.ContainsValue(instrument))
                throw new ArgumentException("Ini files do not support difficulty for this instrument.");

            string key = difficultyKeys.Keys.First(k => difficultyKeys[k] == instrument);

            // Get all the lines, replace the right one and rewrite the file
            File.WriteAllLines(path, File.ReadAllLines(path).Replace(l => GetEntry(l).header == key, $"{key} = {value}"));

        }
        /// <summary>
        /// Writes <see cref="Instrument"/> difficulties from a <see cref="Song"/> to a ini file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="song">Song to get the difficulties from</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="IOException"/>
        internal static void WriteDifficulties(string path, Song song)
        {
            // Get all non-difficulty lines based on the non-null instruments
            File.WriteAllLines(path, File.ReadAllLines(path).Where(l => difficultyKeys.ContainsKey(GetEntry(l).header)).Concat(difficultyKeys.Select(p => (p.Key, song.GetInstrument(p.Value))).Where(t => t.Item2 is not null).Select(p => $"{p.Key} = {p.Item2.Difficulty}")));
        }
    }
}
