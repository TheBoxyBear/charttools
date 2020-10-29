using Ini.Net;
using System;
using System.Collections.Generic;

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
        /// Reads metadata from a ini file.
        /// </summary>
        /// <returns>Instance of <see cref="Metadata"/> containing the data in the file</returns>
        /// <exception cref="ArgumentNullException"/>
        internal static Metadata Read(string path)
        {
            IniFile file;

            try { file = new IniFile(path); }
            catch (Exception e) { throw e; }

            Metadata metadata = new Metadata();

            Dictionary<string, string> contents = new Dictionary<string, string>(file.ReadSection(section));

            if (contents.ContainsKey("name"))
                metadata.Title = contents["name"];
            if (contents.ContainsKey("artist"))
                metadata.Artist = contents["artist"];
            if (contents.ContainsKey("album"))
                metadata.Album = contents["album"];
            if (contents.ContainsKey("year"))
                try { metadata.Year = ushort.Parse(contents["year"]); }
                catch { throw new Exception($"Cannot parse year \"{contents["year"]}\" to {metadata.Year.GetType()}."); }
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
                try { metadata.PreviewStart = ushort.Parse(contents["preview_start_time"]); }
                catch { throw new Exception($"Cannot parse preview start \"{contents["preview_start_time"]}\" to {metadata.PreviewStart.GetType()}."); }
            if (contents.ContainsKey("preview_end_time"))
                try { metadata.PreviewEnd = ushort.Parse(contents["preview_end_time"]); }
                catch { throw new Exception($"Cannot parse preview end \"{contents["preview_end_time"]}\" to {metadata.PreviewEnd.GetType()}."); }
            if (contents.ContainsKey("video_start_time"))
                try { metadata.VideoOffset = float.Parse(contents["video_start_time"]); }
                catch { throw new Exception($"Cannot parse video offset \"{contents["video_start_time"]}\" to {metadata.VideoOffset.GetType()}."); }
            if (contents.ContainsKey("delay"))
                try { metadata.AudioOffset = float.Parse(contents["delay"]); }
                catch { throw new Exception($"Cannot parse audio offset \"{contents["delay"]}\" to {metadata.AudioOffset.GetType()}."); }
            if (contents.ContainsKey("loading_text"))
                metadata.LoadingText = contents["loading_text"];

            return metadata;
        }

        /// <summary>
        /// Write metadata to a ini file.
        /// </summary>
        internal static void Write(string path, Metadata metadata)
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
    }
}
