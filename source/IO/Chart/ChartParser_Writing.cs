using ChartTools.SystemExtensions;
using ChartTools.SystemExtensions.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;

namespace ChartTools.IO.Chart
{
    internal partial class ChartParser
    {
        /// <summary>
        /// Writes a song to a chart file
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="song">Song to write</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="SecurityException"/>
        internal static void WriteSong(string path, Song song)
        {
            // Add threads for metadata, synctrack and global events
            List<Task<IEnumerable<string>>> tasks = new List<Task<IEnumerable<string>>>()
            {
                Task.Run(() => GetPartLines("Song", GetMetadataLines(song.Metadata))),
                Task.Run(() => GetPartLines("SyncTrack", GetSyncTrackLines(song.SyncTrack))),
                Task.Run(() => GetPartLines("Events", song.GlobalEvents.Select(e => GetEventLine(e))))
            };

            // Part names of ghl instruments
            IEnumerable<(Instrument<GHLChord>, Instruments)> ghlInstruments = new (Instrument<GHLChord> instrument, Instruments name)[]
            {
                (song.GHLBass, Instruments.GHLBass),
                (song.GHLGuitar, Instruments.GHLGuitar)
            }.Where(i => i.instrument is not null);
            // Part names of standard instruments
            IEnumerable<(Instrument<StandardChord>, Instruments)> standardInstruments = new (Instrument<StandardChord> instrument, Instruments name)[]
            {
                (song.LeadGuitar, Instruments.LeadGuitar),
                (song.RhythmGuitar, Instruments.RhythmGuitar),
                (song.CoopGuitar, Instruments.CoopGuitar),
                (song.Bass, Instruments.Bass),
                (song.Keys, Instruments.Keys)
            }.Where(i => i.instrument is not null);

            // Types used to get difficulty tracks using reflection
            Type drumsType = typeof(Instrument<DrumsChord>), ghlType = typeof(Instrument<GHLChord>), standardType = typeof(Instrument<StandardChord>);

            foreach (Difficulty difficulty in EnumExtensions.GetValues<Difficulty>())
            {
                // Add threads to get the lines for each non-null drums track
                if (song.Drums is not null)
                    tasks.Add(Task.Run(() =>
                    {
                        IEnumerable<string> lines = GetTrackLines((Track<DrumsChord>)drumsType.GetProperty(difficulty.ToString()).GetValue(song.Drums));

                        return lines.Count() > 0 ? GetPartLines(GetFullPartName(Instruments.Drums, difficulty), lines) : lines;
                    }));

                // Add threads to get the lines for each non-null track of each ghl instrument
                foreach ((Instrument<GHLChord> instrument, Instruments name) in ghlInstruments)
                    tasks.Add(Task.Run(() =>
                    {
                        IEnumerable<string> lines = GetTrackLines((Track<GHLChord>)ghlType.GetProperty(difficulty.ToString()).GetValue(instrument));

                        return lines.Count() > 0 ? GetPartLines(GetFullPartName(name, difficulty), lines) : lines;
                    }));
                // Add threads to get the lines for each non-null track of each standard instrument
                foreach ((Instrument<StandardChord> instrument, Instruments name) in standardInstruments)
                    tasks.Add(Task.Run(() =>
                    {
                        IEnumerable<string> lines = GetTrackLines((Track<StandardChord>)standardType.GetProperty(difficulty.ToString()).GetValue(instrument));

                        return lines.Count() > 0 ? GetPartLines(GetFullPartName(name, difficulty), lines) : lines;
                    }));
            }

            // Join lines with line breaks and write to file
            try { File.WriteAllText(path, string.Join('\n', tasks.SelectMany(t => t.Result))); }
            catch { throw; }

            foreach (Task task in tasks)
                task.Dispose();
        }

        /// <inheritdoc cref="ReplaceInstrument{TChord}(string, Instrument{TChord}, Instruments)"/>
        internal static void ReplaceDrums(string path, Instrument<DrumsChord> inst)
        {
            try { ReplaceInstrument(path, inst, Instruments.Drums); }
            catch { throw; }
        }
        /// <inheritdoc cref="ReplaceInstrument{TChord}(string, Instrument{TChord}, Instruments)"/>
        internal static void ReplaceInstrument(string path, Instrument<GHLChord> inst, GHLInstrument instrument)
        {
            if (!Enum.IsDefined(typeof(GHLInstrument), instrument))
                throw IOExceptions.GetUndefinedInstrumentException();

            try { ReplaceInstrument(path, inst, (Instruments)instrument); }
            catch { throw; }
        }
        /// <inheritdoc cref="ReplaceInstrument{TChord}(string, Instrument{TChord}, Instruments)"/>
        internal static void ReplaceInstrument(string path, Instrument<StandardChord> inst, StandardInstrument instrument)
        {
            if (!Enum.IsDefined(typeof(StandardInstrument), instrument))
                throw IOExceptions.GetUndefinedInstrumentException();

            try { ReplaceInstrument(path, inst, (Instruments)instrument); }
            catch { throw; }
        }
        /// <summary>
        /// Replaces an instrument in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="inst">Instrument to use as a replacement</param>
        /// <param name="instrument">Instrument to replace</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="SecurityException"/>
        private static void ReplaceInstrument<TChord>(string path, Instrument<TChord> inst, Instruments instrument) where TChord : Chord
        {
            if (inst is null)
                throw new ArgumentNullException("Instrument is null.");

            // Tasks that generate the lines and associated part name to write for each track
            List<Task<(IEnumerable<string> lines, string partName)>> tasks = new List<Task<(IEnumerable<string>, string)>>();
            Type instrumentType = typeof(Instrument<TChord>);
            IEnumerable<Difficulty> difficulties = EnumExtensions.GetValues<Difficulty>();

            foreach (Difficulty difficulty in difficulties)
            {
                // Get the track to write
                object track = instrumentType.GetProperty(difficulty.ToString()).GetValue(inst);

                if (track is not null)
                {
                    string partName = GetFullPartName(instrument, difficulty);

                    // Add thread to write the trck
                    tasks.Add(Task.Run(() => (GetPartLines(partName, GetTrackLines(track as Track<TChord>)), partName)));
                }
            }

            try { Task.WaitAll(tasks.ToArray()); }
            catch { throw; }

            try
            {
                if (File.Exists(path))
                    // Get the existing lines, remove lines relating to the instrument's tracks, construct the new parts, insert and rewrite the file
                    File.WriteAllText(path, string.Join('\n', GetLines(path).ReplaceSections(true, tasks.Select<Task<(IEnumerable<string> lines, string partName)>, (IEnumerable<string>, Predicate<string>, Predicate<string>)>(t => (t.Result.lines, l => l == $"[{t.Result.partName}]", l => l == "}")).ToArray())));
                else
                    // Create a new file containing the generated lines
                    File.WriteAllText(path, string.Join('\n', tasks.SelectMany(t => t.Result.lines)));
            }
            catch { throw; }

            foreach (Task task in tasks)
                task.Dispose();
        }

        /// <summary>
        /// Replaces the metadaa in a file.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        /// <param name="metadata">Metadata to write</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="SecurityException"/>
        internal static void ReplaceMetadata(string path, Metadata metadata)
        {
            try { ReplacePart(path, GetMetadataLines(metadata), "Song"); }
            catch { throw; }
        }
        /// <summary>
        /// Replaces the global events in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="events">Events to use as a replacement</param>
        internal static void ReplaceGlobalEvents(string path, IEnumerable<GlobalEvent> events)
        {
            try { ReplacePart(path, events.Select(e => GetEventLine(e)), "Events"); }
            catch { throw; }
        }
        /// <summary>
        /// Replaces the sync track in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="syncTrack">Sync track to write</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="SecurityException"/>
        internal static void ReplaceSyncTrack(string path, SyncTrack syncTrack)
        {
            try { ReplacePart(path, GetSyncTrackLines(syncTrack), "SyncTrack"); }
            catch { throw; }
        }

        /// <summary>
        /// <inheritdoc cref="ReplaceTrack{TChord}(string, Track{TChord}, string)"/>
        /// </summary>
        /// <param name="path">Difficulty of the track to replace</param>
        /// <param name="track">Track to use as a replacement</param>
        /// <param name="difficulty">Difficulty of the track to replace</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="SecurityException"/>
        internal static void ReplaceDrumsTrack(string path, Track<DrumsChord> track, Difficulty difficulty)
        {
            try { ReplaceTrack(path, track, GetFullPartName(Instruments.Drums, difficulty)); }
            catch { throw; }
        }
        /// <summary>
        /// <inheritdoc cref="ReplaceTrack{TChord}(string, Track{TChord}, string)"/>
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="track">Track to use as a replacement</param>
        /// <param name="instrument">Instrument of the track to replace</param>
        /// <param name="difficulty">Difficulty of the track to replace</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="SecurityException"/>
        internal static void ReplaceTrack(string path, Track<GHLChord> track, GHLInstrument instrument, Difficulty difficulty)
        {
            try { ReplaceTrack(path, track, GetFullPartName((Instruments)instrument, difficulty)); }
            catch { throw; }
        }
        /// <summary>
        /// <inheritdoc cref="ReplaceTrack{TChord}(string, Track{TChord}, string)"/>
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="track">Track to use as a replacement</param>
        /// <param name="instrument">Instrument of the track to replace</param>
        /// <param name="difficulty">Difficulty of the track to replace</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="SecurityException"/>
        internal static void ReplaceTrack(string path, Track<StandardChord> track, StandardInstrument instrument, Difficulty difficulty)
        {
            try { ReplaceTrack(path, track, GetFullPartName((Instruments)instrument, difficulty)); }
            catch { throw; }
        }
        /// <summary>
        /// Replaces a track in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="track">Track to use as a replacement</param>
        /// <param name="partName">Name of the part containing the track to replace</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="SecurityException"/>
        private static void ReplaceTrack<TChord>(string path, Track<TChord> track, string partName) where TChord : Chord
        {
            try { ReplacePart(path, GetTrackLines(track), partName); }
            catch { throw; }
        }

        /// <summary>
        /// Replaces a part in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="partContent">Lines representing the entries in the part to use as a replacement</param>
        /// <param name="partName">Name of the part to replace</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="SecurityException"/>
        internal static void ReplacePart(string path, IEnumerable<string> partContent, string partName)
        {
            IEnumerable<string> part = GetPartLines(partName, partContent);

            try
            {
                if (File.Exists(path))
                    File.WriteAllText(path, string.Join('\n', GetLines(path).ReplaceSection(part, l => l == $"[{partName}]", l => l == "}", true)));
                else
                    File.WriteAllText(path, string.Join('\n', part));
            }
            catch { throw; }
        }

        /// <summary>
        /// Gets the lines to write for a difficulty track.
        /// </summary>
        /// <returns>Enumerable of all the lines</returns>
        /// <param name="track">Track to get the lines of</param>
        private static IEnumerable<string> GetTrackLines<TChord>(Track<TChord> track) where TChord : Chord
        {
            if (track is null)
                yield break;

            // Loop through chords, local events and star power, picked using the lowest position
            foreach (TrackObject trackObject in new Collections.Alternating.OrderedAlternatingEnumerable<TrackObject, uint>(t => t.Position, track.Chords, track.LocalEvents, track.StarPower))
            {
                if (trackObject.IsCast(out TChord chord))
                    foreach (string value in chord.GetChartData())
                        yield return GetLine(trackObject.Position.ToString(), value);
                else if (trackObject.IsCast(out LocalEvent e))
                    yield return GetEventLine(e);
                else if (trackObject.IsCast(out StarPowerPhrase phrase))
                    yield return GetLine(trackObject.Position.ToString(), $"P {phrase.Length}");
            }
        }
        /// <summary>
        /// Gets the lines to write for metadata.
        /// </summary>
        /// <returns>Enumerable of all the lines</returns>
        /// <param name="metadata">Metadata to get the lines of</param>
        private static IEnumerable<string> GetMetadataLines(Metadata metadata)
        {
            if (metadata is null)
                yield break;

            if (metadata.Title is not null)
                yield return GetLine("Name", $"\"{metadata.Title}\"");
            if (metadata.Artist is not null)
                yield return GetLine("Artist", $"\"{metadata.Artist}\"");
            if (metadata.Charter is not null && metadata.Charter.Name is not null)
                yield return GetLine("Charter", $"\"{metadata.Charter.Name}\"");
            if (metadata.Album is not null)
                yield return GetLine("Album", $"\"{metadata.Album}\"");
            if (metadata.Year is not null)
                yield return GetLine("Year", $"\", {metadata.Year}\"");
            if (metadata.AudioOffset is not null)
                yield return GetLine("Offset", metadata.AudioOffset.ToString());
            yield return GetLine("Resolution", metadata.Resolution.ToString());
            if (metadata.Difficulty is not null)
                yield return GetLine("Difficulty", metadata.Difficulty.ToString());
            if (metadata.PreviewStart is not null)
                yield return GetLine("PreviewStart", metadata.PreviewStart.ToString());
            if (metadata.PreviewEnd is not null)
                yield return GetLine("PreviewEnd", metadata.PreviewEnd.ToString());
            if (metadata.Genre is not null)
                yield return GetLine("Genre", $"\"{metadata.Genre}\"");
            if (metadata.MediaType is not null)
                yield return GetLine("MetiaType", $"\"{metadata.MediaType}\"");

            // Audio streams
            if (metadata.Streams is not null)
                foreach (PropertyInfo property in typeof(StreamCollection).GetProperties())
                {
                    string value = (string)property.GetValue(metadata.Streams);

                    if (value is not null)
                        yield return GetLine($"{property.Name}Stream", $"\"{value}\"");
                }
        }
        /// <summary>
        /// Gets a line to write for an event.
        /// </summary>
        /// <param name="e">Event to get the line of</param>
        private static string GetEventLine(Event e) => GetLine(e.Position.ToString(), e.Argument == string.Empty ? $"E \"{e.EventTypeString}\"" : $"E \"{e.EventTypeString} {e.Argument}\"");
        /// <summary>
        /// Gets the lines to write for a sync track.
        /// </summary>
        /// <returns>Enumerable of all the lines</returns>
        /// <param name="syncTrack">Sync track to get the liens of</param>
        private static IEnumerable<string> GetSyncTrackLines(SyncTrack syncTrack)
        {
            if (syncTrack is null)
                yield break;

            // Loop through time signatures and tempo markers, picked using the lowest position
            foreach (TrackObject trackObject in new Collections.Alternating.OrderedAlternatingEnumerable<TrackObject, uint>(t => t.Position, syncTrack.TimeSignatures, syncTrack.Tempo))
            {
                if (trackObject is TimeSignature)
                {
                    TimeSignature signature = trackObject as TimeSignature;
                    byte writtenDenominator = (byte)(signature.Denominator / 4);

                    yield return GetLine(trackObject.Position.ToString(), writtenDenominator == 1 ? $"TS {signature.Numerator}" : $"TS {signature.Numerator} {writtenDenominator}");
                }
                else if (trackObject is Tempo)
                {
                    Tempo tempo = trackObject as Tempo;

                    if (tempo.Anchor is not null)
                        yield return GetLine(tempo.Position.ToString(), $"A {GetWrittenFloat((float)tempo.Anchor)}");
                    yield return GetLine(tempo.Position.ToString(), $"B {GetWrittenFloat(tempo.Value)}");
                }
            }
        }

        /// <summary>
        /// Gets the written value of a float.
        /// </summary>
        /// <param name="value">Value to get the written equivalent of</param>
        private static string GetWrittenFloat(float value) => ((int)(value * 1000)).ToString().Replace(".", "").Replace(",", "");
        /// <summary>
        /// Gets the lines to write for a part.
        /// </summary>
        /// <returns>Enumerable of all the lines</returns>
        /// <param name="partName">Name of the part to get the lines of</param>
        /// <param name="lines">Lines in the file</param>
        private static IEnumerable<string> GetPartLines(string partName, IEnumerable<string> lines)
        {
            yield return $"[{partName}]";
            yield return "{";

            foreach (string line in lines)
                yield return $"{line}";

            yield return "}";
        }
        /// <summary>
        /// Gets a line to write from a header and value.
        /// </summary>
        /// <param name="header">Part of the line before the equal sign</param>
        /// <param name="value">Part of the line after the equal sign</param>
        private static string GetLine(string header, string value) => $"  {header} = {value}";
        /// <summary>
        /// Gets the written data for a note.
        /// </summary>
        /// <param name="index">Value of <see cref="Note.NoteIndex"/></param>
        /// <param name="sustain">Value of <see cref="Note.SustainLength"/></param>
        internal static string GetNoteData(byte index, uint sustain) => $"N {index} {sustain}";
    }
}
