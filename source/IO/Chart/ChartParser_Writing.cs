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
        internal static readonly WritingConfiguration DefaultWriteConfig = new()
        {
            SoloNoStarPowerPolicy = SoloNoStarPowerPolicy.Convert,
            EventSource = TrackObjectSource.Seperate,
            StarPowerSource = TrackObjectSource.Seperate
        };

        /// <summary>
        /// Writes a song to a chart file
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="song">Song to write</param>
        /// <inheritdoc cref="ReplacePart(string, IEnumerable{string}, string)" path="/exception"/>
        internal static void WriteSong(string path, Song song, WritingConfiguration? config)
        {
            if (song is null)
                return;

            // Add threads for metadata, sync track and global events
            List<Task<IEnumerable<string>>> tasks = new()
            {
                Task.Run(() => GetPartLines("Song", GetMetadataLines(song.Metadata))),
                Task.Run(() => GetPartLines("SyncTrack", GetSyncTrackLines(song.SyncTrack, config))),
                Task.Run(() => GetPartLines("Events", song.GlobalEvents?.Select(e => GetEventLine(e)))),
                Task.Run(() => GetInstrumentLines(song.Drums, Instruments.Drums, config))
            };

            tasks.AddRange((new (Instrument<GHLChord>? instrument, Instruments name)[]
            {
            (song.GHLBass, Instruments.GHLBass),
            (song.GHLGuitar, Instruments.GHLGuitar)
            }).Select(t => Task.Run(() => GetInstrumentLines(t.instrument, t.name, config))));
            tasks.AddRange((new (Instrument<StandardChord>? instrument, Instruments name)[]
            {
            (song.LeadGuitar, Instruments.LeadGuitar),
            (song.RhythmGuitar, Instruments.RhythmGuitar),
            (song.CoopGuitar, Instruments.CoopGuitar),
            (song.Bass, Instruments.Bass),
            (song.Keys, Instruments.Keys)
            }).Select(t => Task.Run(() => GetInstrumentLines(t.instrument, t.name, config))));
            Task.WaitAll(tasks.ToArray());

            using StreamWriter writer = new(new FileStream(path, FileMode.Create));

            foreach (string line in tasks.SelectMany(t => t.Result))
                writer.WriteLine(line);

            foreach (Task task in tasks)
            {
                task.Wait();
                task.Dispose();
            }
        }

        /// <summary>Replaces drums in a chart file.</summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="inst">Instrument object to write</param>
        /// <inheritdoc cref="ReplaceInstrument{TChord}(string, Instrument{TChord}, Instruments, WritingConfiguration)" path="/exception"/>
        internal static void ReplaceDrums(string path, Instrument<DrumsChord> inst, WritingConfiguration? config) => ReplaceInstrument(path, inst, Instruments.Drums, config);
        /// <summary>Replaces a GHL instrument in a chart file.</summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="data">Tuple containing the Instrument object to write and the instrument to assign it to</param>
        /// <inheritdoc cref="ReplaceInstrument{TChord}(string, Instrument{TChord}, Instruments, WritingConfiguration)" path="/exception"/>
        internal static void ReplaceInstrument(string path, (Instrument<GHLChord> inst, GHLInstrument instEnum) data, WritingConfiguration? config)
        {
            if (!Enum.IsDefined(data.instEnum))
                throw CommonExceptions.GetUndefinedException(data.instEnum);

            ReplaceInstrument(path, data.inst, (Instruments)data.instEnum, config);
        }
        /// <summary>Replaces a standard instrument in a chart file.</summary>
        /// <param name="data">Tuple containing the Instrument object to write and the instrument to assign it to</param>
        /// <inheritdoc cref="ReplaceInstrument{TChord}(string, Instrument{TChord}, Instruments, WritingConfiguration)" path="/param"/>
        /// <inheritdoc cref="ReplaceInstrument{TChord}(string, Instrument{TChord}, Instruments, WritingConfiguration)" path="/exception"/>
        internal static void ReplaceInstrument(string path, (Instrument<StandardChord> inst, StandardInstrument instEnum) data, WritingConfiguration? config)
        {
            if (!Enum.IsDefined(data.instEnum))
                throw CommonExceptions.GetUndefinedException(data.instEnum);

            ReplaceInstrument(path, data.inst, (Instruments)data.instEnum, config);
        }
        /// <summary>
        /// Replaces an instrument in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="inst">Instrument to use as a replacement</param>
        /// <param name="instEnum">Instrument to replace</param>
        /// <exception cref="ArgumentNullException"/>
        /// <inheritdoc cref="ReplacePart(string, IEnumerable{string}, string)" path="/exception"/>
        private static void ReplaceInstrument<TChord>(string path, Instrument<TChord> inst, Instruments instEnum, WritingConfiguration? config) where TChord : Chord
        {
            if (inst is null)
                throw new ArgumentNullException(nameof(inst));

            // Get the instrument lines, combiner them with the lines from the file not related to the instrument and re-write the file
            WriteFile(path, GetInstrumentLines(inst, instEnum, config).Concat(ReadFile(path).RemoveSections(Enum.GetValues<Difficulty>().Select(d => ((Predicate<string>)(l => l == $"[{GetFullPartName(instEnum, d)}]"), (Predicate<string>)(l => l == "}"))).ToArray()).ToArray()));
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
        internal static void ReplaceGlobalEvents(string path, IEnumerable<GlobalEvent> events, WritingConfiguration? config) => ReplacePart(path, events.Select(e => GetEventLine(e)), "Events");
        /// <summary>
        /// Replaces the sync track in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="syncTrack">Sync track to write</param>
        /// <inheritdoc cref="ReplacePart(string, IEnumerable{string}, string)" path="/exception"/>
        internal static void ReplaceSyncTrack(string path, SyncTrack syncTrack, WritingConfiguration? config) => ReplacePart(path, GetSyncTrackLines(syncTrack, config), "SyncTrack");
        /// <summary>
        /// Replaces a track in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="track">Track to use as a replacement</param>
        /// <param name="partName">Name of the part containing the track to replace</param>
        /// <inheritdoc cref="ReplacePart(string, IEnumerable{string}, string)" path="/exception"/>
        internal static void ReplaceTrack<TChord>(string path, (Track<TChord> track, Instruments instrument, Difficulty difficulty) data, WritingConfiguration? config) where TChord : Chord => ReplacePart(path, GetTrackLines(data.track, config), GetFullPartName(data.instrument, data.difficulty));

        /// <summary>
        /// Replaces a part in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="partContent">Lines representing the entries in the part to use as a replacement</param>
        /// <param name="partName">Name of the part to replace</param>
        /// <inheritdoc cref="File.WriteAllText(string, string?)" path="/exception"/>
        /// <inheritdoc cref="ReadFile(string)" path="/exception"/>
        internal static void ReplacePart(string path, IEnumerable<string> partContent, string partName)
        {
            IEnumerable<string> part = GetPartLines(partName, partContent);

            WriteFile(path, (File.Exists(path)
                ? ReadFile(path).ReplaceSection(part, l => l == $"[{partName}]", l => l == "}", true)
                : part).ToArray());
        }

        /// <summary>
        /// Gets the lines to write all the parts making up an instrument.
        /// </summary>
        /// <param name="instrument">Instrument to get the lines for</param>
        /// <param name="instEnum">Instrument to define the part names</param>
        /// <returns>Enumerable of all the lines making up the parts for the instrument</returns>
        private static IEnumerable<string> GetInstrumentLines<TChord>(Instrument<TChord>? instrument, Instruments instEnum, WritingConfiguration? config) where TChord : Chord
        {
            if (instrument is null)
                yield break;

            config ??= DefaultWriteConfig;

            // Apply sharing policies
            instrument.ShareLocalEvents(config.EventSource);
            instrument.ShareStarPower(config.StarPowerSource);

            foreach (Difficulty diff in Enum.GetValues<Difficulty>())
            {
                Track<TChord>? track = instrument.GetTrack(diff);

                if (track is not null)
                {
                    string[] trackLines = GetTrackLines(track, config).ToArray();

                    if (trackLines.Any())
                        foreach (string line in GetPartLines(GetFullPartName(instEnum, diff), trackLines))
                            yield return line;
                }
            }
        }

        /// <summary>
        /// Gets the lines to write for a difficulty track.
        /// </summary>
        /// <returns>Enumerable of all the lines making up the inside of the part</returns>
        /// <param name="track">Track to get the lines of</param>
        private static IEnumerable<string> GetTrackLines<TChord>(Track<TChord> track, WritingConfiguration? config) where TChord : Chord
        {
            if (track is null)
                yield break;

            config ??= DefaultWriteConfig;

            // Convert solo and soloend events into star power
            if (config.SoloNoStarPowerPolicy == SoloNoStarPowerPolicy.Convert && track.StarPower.Count == 0)
            {
                StarPowerPhrase? starPower = null;

                if (track.LocalEvents is null)
                    yield break;

                foreach (LocalEvent e in track.LocalEvents)
                    switch (e.EventType)
                    {
                        case LocalEventType.Solo:
                            if (starPower is not null)
                            {
                                starPower.Length = e.Position - starPower.Position;
                                track.StarPower.Add(starPower);
                            }

                            starPower = new(e.Position);
                            break;
                        case LocalEventType.SoloEnd when starPower is not null:

                            starPower.Length = e.Position - starPower.Position;
                            track.StarPower.Add(starPower);

                            starPower = null;
                            break;
                    }

                track.LocalEvents.RemoveWhere(e => e.EventType is LocalEventType.Solo or LocalEventType.SoloEnd);
            }

            // Loop through chords, local events and star power, picked using the lowest position
            foreach (TrackObject trackObject in new Collections.Alternating.OrderedAlternatingEnumerable<uint, TrackObject>(t => t.Position, track.Chords, track.LocalEvents, track.StarPower))
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
        private static IEnumerable<string> GetMetadataLines(Metadata? metadata)
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
                yield return GetLine("Offset", metadata.AudioOffset.ToString()!);
            yield return GetLine("Resolution", metadata.Resolution.ToString());
            if (metadata.Difficulty is not null)
                yield return GetLine("Difficulty", metadata.Difficulty.ToString()!);
            if (metadata.PreviewStart is not null)
                yield return GetLine("PreviewStart", metadata.PreviewStart.ToString()!);
            if (metadata.PreviewEnd is not null)
                yield return GetLine("PreviewEnd", metadata.PreviewEnd.ToString()!);
            if (metadata.Genre is not null)
                yield return GetLine("Genre", $"\"{metadata.Genre}\"");
            if (metadata.MediaType is not null)
                yield return GetLine("MetiaType", $"\"{metadata.MediaType}\"");

            // Audio streams
            if (metadata.Streams is not null)
                foreach (PropertyInfo property in typeof(StreamCollection).GetProperties())
                {
                    string value = (string)property.GetValue(metadata.Streams)!;

                    if (value is not null)
                        yield return GetLine($"{property.Name}Stream", $"\"{value}\"");
                }

            if (metadata.UnidentifiedData is not null)
                foreach (MetadataItem data in metadata.UnidentifiedData.Where(d => d.Origin == FileFormat.Chart))
                    yield return GetLine(data.Key, data.Data);
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
        private static IEnumerable<string> GetSyncTrackLines(SyncTrack? syncTrack, WritingConfiguration? config)
        {
            if (syncTrack is null)
                yield break;

            config ??= DefaultWriteConfig;

            // Loop through time signatures and tempo markers, picked using the lowest position
            foreach (TrackObject trackObject in new Collections.Alternating.OrderedAlternatingEnumerable<uint, TrackObject>(t => t.Position, syncTrack.TimeSignatures, syncTrack.Tempo))
            {
                if (trackObject is TimeSignature ts)
                {
                    byte writtenDenominator = (byte)Math.Log2(ts.Denominator);

                    yield return GetLine(trackObject.Position.ToString(), writtenDenominator == 1 ? $"TS {ts.Numerator}" : $"TS {ts.Numerator} {writtenDenominator}");
                }
                else if (trackObject is Tempo tempo)
                {
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
        private static IEnumerable<string> GetPartLines(string partName, IEnumerable<string>? lines)
        {
            if (lines is null || !lines.Any())
                yield break;

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
        private static string GetLine(string header, string value) => value is null ? string.Empty : $"  {header} = {value}";
        /// <summary>
        /// Gets the written data for a note.
        /// </summary>
        /// <param name="index">Value of <see cref="Note.NoteIndex"/></param>
        /// <param name="sustain">Value of <see cref="Note.SustainLength"/></param>
        internal static string GetNoteData(byte index, uint sustain) => $"N {index} {sustain}";

        private static void WriteFile(string path, IEnumerable<string> lines)
        {
            using StreamWriter writer = new(new FileStream(path, FileMode.Create));

            foreach (string line in lines)
                writer.WriteLine(line);
        }
    }
}
