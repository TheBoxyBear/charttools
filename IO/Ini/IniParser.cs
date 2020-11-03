using Ini.Net;
using System;
using System.Collections.Generic;
using System.IO;

namespace ChartTools.IO.Ini
{
    /// <summary>
    /// Provides methods for reading and writing ini files
    /// </summary>
    internal static class IniParser
    {
        /// <summary>
        /// Name of the section to read
        /// </summary>
        private const string section = "song";
        /// <summary>
        /// Keys for <see cref="Instrument"/> difficulties
        /// </summary>
        private static readonly Dictionary<Instruments, string> difficultyKeys = new Dictionary<Instruments, string>()
        {
            { Instruments.Drums, "diff_drums" },
            { Instruments.GHLGuitar, "diff_guitarghl" },
            { Instruments.GHLBass, "diff_bassghl" },
            { Instruments.LeadGuitar, "diff_guitar" },
            { Instruments.Bass, "diff_bass" },
            { Instruments.Keys, "diff_keys" }
        };

        /// <summary>
        /// Reads metadata from a ini file.
        /// </summary>
        /// <returns>Instance of <see cref="Metadata"/> containing the data in the file</returns>
        /// <exception cref="FormatException"/>
        internal static Metadata ReadMetadata(string path)
        {
            IniFile file;

            try { file = new IniFile(path); }
            catch { throw; }

            Metadata metadata = new Metadata();

            Dictionary<string, string> contents = new Dictionary<string, string>(file.ReadSection(section));

            if (contents.ContainsKey("name"))
                metadata.Title = contents["name"];
            if (contents.ContainsKey("artist"))
                metadata.Artist = contents["artist"];
            if (contents.ContainsKey("album"))
                metadata.Album = contents["album"];
            if (contents.ContainsKey("year"))
                metadata.Year = ushort.TryParse(contents["year"], out ushort value)
                    ? value
                    : throw new FormatException($"Cannot parse year \"{contents["year"]}\" to {metadata.Year.GetType()}.");
            if (contents.ContainsKey("genre"))
                metadata.Genre = contents["genre"];
            if (contents.ContainsKey("charter"))
            {
                if (metadata.Charter is null)
                    metadata.Charter = new Charter();
                metadata.Charter.Name = contents["charter"];
            }
            if (contents.ContainsKey("icon"))
            {
                if (metadata.Charter is null)
                    metadata.Charter = new Charter();
                metadata.Charter.Icon = contents["icon"];
            }
            if (contents.ContainsKey("preview_start_time"))
                metadata.PreviewStart = ushort.TryParse(contents["preview_start_time"], out ushort value)
                    ? value
                    : throw new FormatException($"Cannot parse preview start \"{contents["preview_start_time"]}\" to {metadata.PreviewStart.GetType()}.");
            if (contents.ContainsKey("preview_end_time"))
                metadata.PreviewEnd = ushort.TryParse(contents["preview_end_time"], out ushort value)
                    ? value
                    : throw new FormatException($"Cannot parse preview end \"{contents["preview_end_time"]}\" to {metadata.PreviewEnd.GetType()}.");
            if (contents.ContainsKey("video_start_time"))
                try { metadata.VideoOffset = float.Parse(contents["video_start_time"]); }
                catch { throw new FormatException($"Cannot parse video offset \"{contents["video_start_time"]}\" to {metadata.VideoOffset.GetType()}."); }
            if (contents.ContainsKey("delay"))
                try { metadata.AudioOffset = float.Parse(contents["delay"]); }
                catch { throw new FormatException($"Cannot parse audio offset \"{contents["delay"]}\" to {metadata.AudioOffset.GetType()}."); }
            if (contents.ContainsKey("loading_text"))
                metadata.LoadingText = contents["loading_text"];

            return metadata;
        }
        /// <summary>
        /// Writes metadata to a ini file.
        /// </summary>
        internal static void WriteMetadata(string path, Metadata metadata)
        {
            IniFile file = new IniFile(path);

            file.WriteString(section, "name", metadata.Title);
            file.WriteString(section, "artist", metadata.Artist);
            file.WriteString(section, "album", metadata.Album);
            file.WriteInteger(section, "year", metadata.Year.GetValueOrDefault(0));
            file.WriteString(section, "genre", metadata.Genre);
            file.WriteString(section, "charter", metadata.Charter.Name);
            file.WriteString(section, "icon", metadata.Charter.Icon);
            file.WriteDecimal(section, "preview_start_time", metadata.PreviewStart.GetValueOrDefault(0));
            file.WriteDecimal(section, "preview_end_time", metadata.PreviewEnd.GetValueOrDefault(0));
            file.WriteFloat(section, "video_start_time", metadata.VideoOffset.GetValueOrDefault(0));
            file.WriteFloat(section, "delay", metadata.AudioOffset.GetValueOrDefault(0));
        }

        /// <summary>
        /// Reads an <see cref="Instrument"/> difficulty from a ini file.
        /// </summary>
        /// <returns>Difficulty read
        ///     <para><see langword="null"/> if the file does not mentio na difficulty for the provided instrument</para>
        /// </returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        internal static sbyte? ReadDifficulty(string path, Instruments instrument)
        {
            if (!difficultyKeys.ContainsKey(instrument))
                throw new ArgumentException("Ini files do not support difficulty for this instrument.");

            try { return ReadDifficulty(new IniFile(path).ReadSection(section), difficultyKeys[instrument]); }
            catch (IndexOutOfRangeException) { throw new IOException("Cannot read file."); }
            catch { throw; }
        }
        /// <summary>
        /// Reads an <see cref="Instrument"/> difficulty from the contents of a ini file.
        /// </summary>
        /// <returns><inheritdoc cref="ReadDifficulty(string, Instruments)"/></returns>
        /// <exception cref="FormatException"/>
        private static sbyte? ReadDifficulty(IDictionary<string, string> contents, string key) => !contents.ContainsKey(key)
                ? null
                : (sbyte?)(!sbyte.TryParse(contents[key], out sbyte difficulty)
                ? throw new FormatException($"Cannot parse difficulty {contents[key]} to byte.")
                : difficulty);
        /// <summary>
        /// Reads <see cref="Instrument"/> difficulties from a ini file and assigns them to the instruments in a <see cref="Song"/>.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="FormatException"/>
        internal static void ReadDifficulties(string path, Song song)
        {
            IniFile file;

            try { file = new IniFile(path); }
            catch { throw; }

            Dictionary<string, string> contents = new Dictionary<string, string>(file.ReadSection(section));

            foreach (Instruments instrument in new Instruments[]
            {
                Instruments.Drums,
                Instruments.GHLGuitar,
                Instruments.GHLBass,
                Instruments.LeadGuitar,
                Instruments.Bass,
                Instruments.Keys
            })
            {
                Instrument inst = song.GetInstrument(instrument);
                string key = difficultyKeys[instrument];

                if (inst is not null && contents.ContainsKey(key))
                    try { inst.Difficulty = ReadDifficulty(contents, key); }
                    catch { throw; }
            }
        }

        /// <summary>
        /// Writes an <see cref="Instrument"/> difficulty to a ini file.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="FormatException"/>
        internal static void WriteDifficulty(string path, Instruments instrument, sbyte value)
        {
            if (!difficultyKeys.ContainsKey(instrument))
                throw new ArgumentException("Ini files do not support difficulty for this instrument.");

            IniFile file = new IniFile(path);

            try { WriteDifficulty(file, file.ReadSection(section), difficultyKeys[instrument], value); }
            catch (IndexOutOfRangeException) { throw new IOException("Cannot read file."); }
            catch { throw; }
        }
        /// <inheritdoc cref="WriteDifficulty(string, Instruments, sbyte)"/>
        private static void WriteDifficulty(IniFile file, IDictionary<string, string> contents, string key, sbyte value)
        {
            if (contents.ContainsKey(key))
                file.DeleteKey(section, key);

            file.WriteInteger(section, key, value);
        }
        /// <summary>
        /// Writes <see cref="Instrument"/> difficulties from a <see cref="Song"/> to a ini file.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="IOException"/>
        internal static void WriteDifficulties(string path, Song song)
        {
            IniFile file = new IniFile(path);

            Dictionary<string, string> contents = new Dictionary<string, string>(file.ReadSection(section));

            foreach (Instruments instrument in new Instruments[]
            {
                Instruments.Drums,
                Instruments.GHLGuitar,
                Instruments.GHLBass,
                Instruments.LeadGuitar,
                Instruments.Bass,
                Instruments.Keys
            })
            {
                Instrument inst = song.GetInstrument(instrument);

                if (inst is not null && inst.Difficulty is not null)
                    try { WriteDifficulty(file, contents, difficultyKeys[instrument], inst.Difficulty.Value); }
                    catch { throw; }
            }
        }
    }
}
