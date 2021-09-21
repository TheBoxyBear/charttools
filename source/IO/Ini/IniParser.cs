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
            { nameof(Metadata.Title), "name" },
            { nameof(Metadata.Artist), "artist" },
            { nameof(Metadata.Album), "album" },
            { nameof(Metadata.AlbumTrack), "album_track" },
            { nameof(Metadata.PlaylistTrack), "playlist_track" },
            { nameof(Metadata.Genre), "genre" },
            { nameof(Metadata.Year), "year" },
            { nameof(Metadata.PreviewStart), "preview_start_time" },
            { nameof(Metadata.PreviewEnd), "preview_end_time" },
            { nameof(Metadata.AudioOffset), "delay" },
            { nameof(Metadata.VideoOffset), "video_start_time" },
            { nameof(Metadata.Length), "song_length" },
            { nameof(Metadata.IsModchart), "modchart" }
        };

        /// <summary>
        /// Reads metadata from a ini file.
        /// </summary>
        /// <returns>Instance of <see cref="Metadata"/> containing the data in the file</returns>
        /// <param name="path">Path of the file to read</param>
        /// <inheritdoc cref="File.ReadLines(string)" path="/exception"/>
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
                    case "album_track" or "track":
                        metadata.AlbumTrack = ushort.TryParse(value, out ushort ushortValue) ? ushortValue
                            : throw new FormatException($"Cannot parse album track \"{value}\" to ushort.");
                        break;
                    case "playlist_track":
                        metadata.PlaylistTrack = ushort.TryParse(value, out ushortValue) ? ushortValue
                            : throw new FormatException($"Cannot parse playlist track \"{value}\" to ushort.");
                        break;
                    case "year":
                        metadata.Year = ushort.TryParse(value, out ushortValue) ? ushortValue
                            : throw new FormatException($"Cannot parse year \"{value}\" to ushort.");
                        break;
                    case "genre":
                        metadata.Genre = value;
                        break;
                    case "charter" or "frets":
                        metadata.Charter ??= new Charter();
                        metadata.Charter.Name = value;
                        break;
                    case "icon":
                        metadata.Charter ??= new();
                        metadata.Charter.Icon = value;
                        break;
                    case "preview_start_time":
                        metadata.PreviewStart = uint.TryParse(value, out uintValue) ? uintValue
                            : throw new FormatException($"Cannot parse preview start \"{value}\" to uint.");
                        break;
                    case "preview_end_time":
                        metadata.PreviewEnd = uint.TryParse(value, out uintValue) ? uintValue
                            : throw new FormatException($"Cannot parse preview end \"{value}\" to uint.");
                        break;
                    case "delay":
                        metadata.AudioOffset = int.TryParse(value, out intValue) ? intValue
                            : throw new FormatException($"Cannot parse audio offset \"{value}\" to int.");
                        break;
                    case "video_start_time":
                        metadata.VideoOffset = int.TryParse(value, out intValue) ? intValue
                            : throw new FormatException($"Cannot parse video offset \"{value}\" to int.");
                        break;
                    case "song_length":
                        metadata.Length = uint.TryParse(value, out uintValue) ? uintValue
                            : throw new FormatException($"Cannot parse song length \"{value}\" to uint.");
                        break;
                    case "loading_text":
                        metadata.LoadingText = value;
                        break;
                    case "modchart":
                        metadata.IsModchart = bool.TryParse(value, out bool boolValue) ? boolValue
                            : throw new FormatException($"Cannot parse modchart \"{value}\" to bool.");
                        break;
                    default:
                        metadata.UnidentifiedData.Add(new() { Key = header, Data = value, Origin = FileFormat.Ini });
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
        internal static void WriteMetadata(string path, Metadata metadata)
        {
            using StreamWriter writer = new(new FileStream(path, FileMode.Create));

            foreach (string line in GetLines(metadata))
                writer.WriteLine(line);

        }

        private static IEnumerable<string> GetLines(Metadata metadata)
        {
            yield return "[Song]";

            if (metadata is null)
                yield break;

            Type metadataType = typeof(Metadata);

            // Get the value of all properties whose name is in the dictionary and pair with its matching key, filtered to non-null properties
            foreach ((string key, object value) in metadataKeys.Keys.Select(p => (metadataKeys[p], metadataType.GetProperty(metadataKeys[p])!.GetValue(metadata))).Where(t => t.Item2 is not null))
                yield return $"{key} = {value}";

            if (metadata.Charter is not null)
            {
                if (metadata.Charter.Name is not null)
                    yield return $"charter = {metadata.Charter.Name}";
                if (metadata.Charter.Icon is not null)
                    yield return $"icon = {metadata.Charter.Icon}";
            }

            if (metadata.UnidentifiedData is not null)
                foreach (MetadataItem data in metadata.UnidentifiedData.Where(d => d.Origin == FileFormat.Ini))
                    yield return $"{data.Key} = {data.Data}";
        }

        /// <summary>
        /// Reads an <see cref="Instrument"/> difficulty from a ini file.
        /// </summary>
        /// <returns>Difficulty read
        ///     <para><see langword="null"/> if the file does not mention a difficulty for the provided instrument</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <param name="instrument">Instrument to read</param>
        /// <inheritdoc cref="File.ReadLines(string)" path="/exception"/>
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
        /// <inheritdoc cref="File.ReadAllLines(string)" path="/exception"/>
        internal static void ReadDifficulties(string path, Song song)
        {
            foreach (string line in File.ReadAllLines(path))
            {
                (string header, string value) = GetEntry(line);

                if (difficultyKeys.ContainsKey(header))
                {
                    Instrument? inst = song.GetInstrument(difficultyKeys[header]);

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
        /// <inheritdoc cref="File.ReadAllLines" path="/exception"/>
        /// <inheritdoc cref="File.WriteAllLines(string, IEnumerable{string})" path="/exception"/>
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
        /// <inheritdoc cref="WriteDifficulty(string, Instruments, sbyte)" path="/exception"/>
        internal static void WriteDifficulties(string path, Song song)
        {
            // Get all non-difficulty lines based on the non-null instruments
            File.WriteAllLines(path,
                File.ReadAllLines(path).Where(l => difficultyKeys.ContainsKey(GetEntry(l).header))
                .Concat(difficultyKeys
                .Select(p => (p.Key, song.GetInstrument(p.Value)))
                .Where(t => t.Item2?.Difficulty is not null).Select(p => $"{p.Key} = {p.Item2!.Difficulty}")));
        }
    }
}
