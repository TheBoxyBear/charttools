using ChartTools.SystemExtensions.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        /// <inheritdoc cref="ReplacePart(string, IEnumerable{string}, string)" path="/exception"/>
        internal static void WriteSong(string path, Song song, WritingConfiguration config)
        {
            // Add threads for metadata, sync track and global events
            List<Task<IEnumerable<string>>> tasks = new()
            {
                Task.Run(() => GetPartLines("Song", GetMetadataLines(song.Metadata))),
                Task.Run(() => GetPartLines("SyncTrack", GetSyncTrackLines(song.SyncTrack))),
                Task.Run(() => GetPartLines("Events", song.GlobalEvents.Select(e => GetEventLine(e))))
            };

            // Part names of GHL instruments
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

            foreach (Difficulty difficulty in Enum.GetValues<Difficulty>())
            {
                // Add threads to get the lines for each non-null drums track
                if (song.Drums is not null)
                    tasks.Add(Task.Run(() =>
                    {
                        IEnumerable<string> lines = GetTrackLines((Track<DrumsChord>)drumsType.GetProperty(difficulty.ToString()).GetValue(song.Drums), config);

                        return lines.Any() ? GetPartLines(GetFullPartName(Instruments.Drums, difficulty), lines) : lines;
                    }));

                // Add threads to get the lines for each non-null track of each GHL instrument
                foreach ((Instrument<GHLChord> instrument, Instruments name) in ghlInstruments)
                    tasks.Add(Task.Run(() =>
                    {
                        IEnumerable<string> lines = GetTrackLines((Track<GHLChord>)ghlType.GetProperty(difficulty.ToString()).GetValue(instrument), config);

                        return lines.Any() ? GetPartLines(GetFullPartName(name, difficulty), lines) : lines;
                    }));
                // Add threads to get the lines for each non-null track of each standard instrument
                foreach ((Instrument<StandardChord> instrument, Instruments name) in standardInstruments)
                    tasks.Add(Task.Run(() =>
                    {
                        IEnumerable<string> lines = GetTrackLines((Track<StandardChord>)standardType.GetProperty(difficulty.ToString()).GetValue(instrument), config);

                        return lines.Any() ? GetPartLines(GetFullPartName(name, difficulty), lines) : lines;
                    }));
            }

            // Join lines with line breaks and write to file
            File.WriteAllText(path, string.Join('\n', tasks.SelectMany(t => t.Result)));

            foreach (Task task in tasks)
                task.Dispose();
        }

        /// <inheritdoc cref="ReplaceInstrument{TChord}(string, Instrument{TChord}, Instruments)"/>
        internal static void ReplaceDrums(string path, Instrument<DrumsChord> inst, WritingConfiguration config) => ReplaceInstrument(path, (inst, Instruments.Drums), config);
        /// <inheritdoc cref="ReplaceInstrument{TChord}(string, Instrument{TChord}, Instruments)"/>
        internal static void ReplaceInstrument(string path, (Instrument<GHLChord> inst, GHLInstrument instrument) data, WritingConfiguration config)
        {
            if (!Enum.IsDefined(data.instrument))
                throw CommonExceptions.GetUndefinedException(data.instrument);

            ReplaceInstrument(path, data, config);
        }
        /// <inheritdoc cref="ReplaceInstrument{TChord}(string, Instrument{TChord}, Instruments)"/>
        internal static void ReplaceInstrument(string path, (Instrument<StandardChord> inst, StandardInstrument instrument) data, WritingConfiguration config)
        {
            if (!Enum.IsDefined(data.instrument))
                throw CommonExceptions.GetUndefinedException(data.instrument);

            ReplaceInstrument(path, data, config);
        }
        /// <summary>
        /// Replaces an instrument in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="inst">Instrument to use as a replacement</param>
        /// <param name="instrument">Instrument to replace</param>
        /// <exception cref="ArgumentNullException"/>
        /// <inheritdoc cref="ReplacePart(string, IEnumerable{string}, string)" path="/exception"/>
        private static void ReplaceInstrument<TChord>(string path, (Instrument<TChord> inst, Instruments instrument) data, WritingConfiguration config) where TChord : Chord
        {
            if (data.inst is null)
                throw new ArgumentNullException("data.inst");

            // Tasks that generate the lines and associated part name to write for each track
            List<Task<(IEnumerable<string> lines, string partName)>> tasks = new();
            Type instrumentType = typeof(Instrument<TChord>);

            foreach (Difficulty difficulty in Enum.GetValues<Difficulty>())
            {
                // Get the track to write
                object track = instrumentType.GetProperty(difficulty.ToString()).GetValue(data.inst);

                if (track is not null)
                {
                    string partName = GetFullPartName(data.instrument, difficulty);

                    // Add thread to write the track
                    tasks.Add(Task.Run(() => (GetPartLines(partName, GetTrackLines(track as Track<TChord>, config)), partName)));
                }
            }

            Task.WaitAll(tasks.ToArray());

            string content = File.Exists(path) ?
                // Get the existing lines, remove lines relating to the instrument's tracks, construct the new parts and insert
                string.Join('\n', GetLines(path).ReplaceSections(true, tasks.Select<Task<(IEnumerable<string> lines, string partName)>, (IEnumerable<string>, Predicate<string>, Predicate<string>)>(t => (t.Result.lines, l => l == $"[{t.Result.partName}]", l => l == "}")).ToArray())) :
                // Get only the generated lines
                string.Join('\n', tasks.SelectMany(t => t.Result.lines));

            File.WriteAllText(path, content);

            foreach (Task task in tasks)
                task.Dispose();
        }

        /// <summary>
        /// Replaces the metadata in a file.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        /// <param name="metadata">Metadata to write</param>
        /// <inheritdoc cref="ReplacePart(string, IEnumerable{string}, string)" path="/exception"/>
        internal static void ReplaceMetadata(string path, Metadata metadata) => ReplacePart(path, GetMetadataLines(metadata), "Song");

        /// <summary>
        /// Replaces the global events in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="events">Events to use as a replacement</param>
        /// <inheritdoc cref="ReplacePart(string, IEnumerable{string}, string)" path="/exception"/>
        internal static void ReplaceGlobalEvents(string path, IEnumerable<GlobalEvent> events, WritingConfiguration config) => ReplacePart(path, events.Select(e => GetEventLine(e)), "Events");
        /// <summary>
        /// Replaces the sync track in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="syncTrack">Sync track to write</param>
        /// <inheritdoc cref="ReplacePart(string, IEnumerable{string}, string)" path="/exception"/>
        internal static void ReplaceSyncTrack(string path, SyncTrack syncTrack) => ReplacePart(path, GetSyncTrackLines(syncTrack), "SyncTrack");
        /// <summary>
        /// Replaces a track in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="track">Track to use as a replacement</param>
        /// <param name="partName">Name of the part containing the track to replace</param>
        /// <inheritdoc cref="ReplacePart(string, IEnumerable{string}, string)" path="/exception"/>
        internal static void ReplaceTrack<TChord>(string path, (Track<TChord> track, Instruments instrument, Difficulty difficulty) data, WritingConfiguration config) where TChord : Chord
        {
            // Convert solo and soloend events into star power
            if (config.SoloNoStarPowerRule == SoloNoStarPowerRule.Convert && data.track.StarPower.Count == 0)
            {
                StarPowerPhrase starPower = null;

                foreach (LocalEvent e in data.track.LocalEvents)
                    switch (e.EventType)
                    {
                        case LocalEventType.Solo:
                            if (starPower is not null)
                            {
                                starPower.Length = e.Position - starPower.Position;
                                data.track.StarPower.Add(starPower);
                            }

                            starPower = new(e.Position);
                            break;
                        case LocalEventType.SoloEnd:
                            starPower.Length = e.Position - starPower.Position;
                            data.track.StarPower.Add(starPower);

                            starPower = null;
                            break;
                    }

                data.track.LocalEvents.RemoveWhere(e => e.EventType is LocalEventType.Solo or LocalEventType.SoloEnd);
            }

            ReplacePart(path, GetTrackLines(data.track, config), GetFullPartName(data.instrument, data.difficulty));
        }

        /// <summary>
        /// Replaces a part in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="partContent">Lines representing the entries in the part to use as a replacement</param>
        /// <param name="partName">Name of the part to replace</param>
        /// <inheritdoc cref="File.WriteAllText(string, string?)" path="/exception"/>
        /// <inheritdoc cref="GetLines(string)" path="/exception"/>
        internal static void ReplacePart(string path, IEnumerable<string> partContent, string partName)
        {
            IEnumerable<string> part = GetPartLines(partName, partContent);
            using StreamWriter writer = new(path);

            foreach (string line in File.Exists(path)
                ? GetLines(path).ReplaceSection(part, l => l == $"[{partName}]", l => l == "}", true)
                : part)
                writer.WriteLine(line);
        }

        /// <summary>
        /// Gets the lines to write for a difficulty track.
        /// </summary>
        /// <returns>Enumerable of all the lines</returns>
        /// <param name="track">Track to get the lines of</param>
        private static IEnumerable<string> GetTrackLines<TChord>(Track<TChord> track, WritingConfiguration config) where TChord : Chord
        {
            if (track is null)
                yield break;

            // Loop through chords, local events and star power, picked using the lowest position
            foreach (TrackObject trackObject in new Collections.Alternating.OrderedAlternatingEnumerable<TrackObject, uint>(t => t.Position, track.Chords, track.LocalEvents, track.StarPower))
                switch (trackObject)
                {
                    case TChord chord:
                        foreach (string value in chord.GetChartData())
                            yield return GetLine(trackObject.Position.ToString(), value);
                        break;
                    case LocalEvent e:
                        yield return GetEventLine(e);
                        break;
                    case StarPowerPhrase phrase:
                        yield return GetLine(trackObject.Position.ToString(), $"P {phrase.Length}");
                        break;
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
        private static string GetEventLine(Event e) => GetLine(e.Position.ToString(), $"E \"{e.EventData}\"");
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
                    byte writtenDenominator = (byte)Math.Log2(signature.Denominator);

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
