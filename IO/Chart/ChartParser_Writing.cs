using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ChartTools.IO.Chart
{
    internal partial class ChartParser
    {
        /// <summary>
        /// Writes a song to a chart file
        /// </summary>
        ///<exception cref="ArgumentException"/>
        ///<exception cref="ArgumentNullException"/>
        ///<exception cref="PathTooLongException"/>
        ///<exception cref="DirectoryNotFoundException"/>
        ///<exception cref="IOException"/>
        ///<exception cref="UnauthorizedAccessException"/>
        ///<exception cref="NotSupportedException"/>
        ///<exception cref="System.Security.SecurityException"/>
        internal static void WriteSong(string path, Song song)
        {
            //Add threads for metadata, synctrack and global events
            List<Task<IEnumerable<string>>> tasks = new List<Task<IEnumerable<string>>>()
            {
                Task.Run(() => GetPartLines("Song", GetMetadataLines(song.Metadata))),
                Task.Run(() => GetPartLines("SyncTrack", GetSyncTrackLines(song.SyncTrack))),
                Task.Run(() => GetPartLines("Events", song.GlobalEvents.Select(e => GetEventLine(e))))
            };

            //Part names of ghl instruments
            IEnumerable<(Instrument<GHLChord>, string)> ghlInstruments = new (Instrument<GHLChord> instrument, string name)[]
            {
                (song.GHLBass, "GHLBass"),
                (song.GHLGuitar, "GHLGuitar")
            }.Where(i => i.instrument is not null);
            //Part names of standard instruments
            IEnumerable<(Instrument<StandardChord>, string)> standardInstruments = new (Instrument<StandardChord> instrument, string name)[]
            {
                (song.LeadGuitar, "Single"),
                (song.RhythmGuitar, "Rythm"),
                (song.CoopGuitar, "Coop"),
                (song.Bass, "Bass"),
                (song.Keys, "Keys")
            }.Where(i => i.instrument is not null);

            //Types used to get difficulty tracks using reflection
            Type drumsType = typeof(Instrument<DrumsChord>), ghlType = typeof(Instrument<GHLChord>), standardType = typeof(Instrument<StandardChord>);

            foreach (string difficulty in EnumExtensions.GetValues<Difficulty>().Select(d => d.ToString()))
            {
                //Add threads to get the lines for each non-null drums track
                if (song.Drums is not null)
                    tasks.Add(Task.Run(() =>
                    {
                        IEnumerable<string> lines = GetTrackLines((Track<DrumsChord>)drumsType.GetProperty(difficulty).GetValue(song.Drums));

                        if (lines.Count() > 0)
                            return GetPartLines(difficulty + "Drums", lines);

                        return lines;
                    }));

                //Add threads to get the lines for each non-null track of each ghl instrument
                foreach ((Instrument<GHLChord> instrument, string name) instrumentTuple in ghlInstruments)
                    tasks.Add(Task.Run(() =>
                    {
                        IEnumerable<string> lines = GetTrackLines((Track<GHLChord>)ghlType.GetProperty(difficulty).GetValue(instrumentTuple.instrument));

                        if (lines.Count() > 0)
                            return GetPartLines(difficulty + instrumentTuple.name, lines) ;

                        return lines;
                    }));
                //Add threads to get the lines for each non-null track of each standard instrument
                foreach ((Instrument<StandardChord> instrument, string name) instrumentTuple in standardInstruments)
                    tasks.Add(Task.Run(() =>
                    {
                        IEnumerable<string> lines = GetTrackLines((Track<StandardChord>)standardType.GetProperty(difficulty).GetValue(instrumentTuple.instrument));

                        if (lines.Count() > 0)
                            return GetPartLines(difficulty + instrumentTuple.name, lines);

                        return lines;
                    }));
            }

            ///<exception cref="ArgumentException"/>
            ///<exception cref="ArgumentNullException"/>
            ///<exception cref="PathTooLongException"/>
            ///<exception cref="DirectoryNotFoundException"/>
            ///<exception cref="IOException"/>
            ///<exception cref="UnauthorizedAccessException"/>
            ///<exception cref="NotSupportedException"/>
            ///<exception cref="System.Security.SecurityException"/>

            //Join lines with line breaks and write to file
            try { File.WriteAllText(path, string.Join('\n', tasks.SelectMany(t => t.Result))); }
            catch (Exception e) { throw e; }

            foreach (Task task in tasks)
                task.Dispose();
        }

        /// <summary>
        /// Gets the lines to write for a difficulty track.
        /// </summary>
        /// <returns>Enumerable of all the lines</returns>
        private static IEnumerable<string> GetTrackLines<TChord>(Track<TChord> track) where TChord : Chord
        {
            if (track is null)
                yield break;

            //Loop through chords, local events and star power, picked using the lowest position
            foreach (TrackObject trackObject in new Collections.Alternating.OrderedAlternatingEnumerable<TrackObject, uint>(t => t.Position, track.Chords, track.LocalEvents, track.StarPowerPhrases))
            {
                if (trackObject is TChord)
                    foreach (string value in (trackObject as TChord).GetChartData())
                        yield return GetLine(trackObject.Position.ToString(), value);
                else if (trackObject is LocalEvent)
                    yield return GetEventLine(trackObject as Event);
                else if (trackObject is StarPowerPhrase)
                    yield return GetLine(trackObject.Position.ToString(), $"P {(trackObject as StarPowerPhrase).Length}");
            }
        }
        /// <summary>
        /// Gets the lines to write for metadata.
        /// </summary>
        /// <returns>Enumerable of all the lines</returns>
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

            //Audio streams
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
        private static string GetEventLine(Event e) => GetLine(e.Position.ToString(), e.Argument == string.Empty ? $"E \"{e.EventTypeString}\"" : $"E \"{e.EventTypeString} {e.Argument}\"");
        /// <summary>
        /// Gets the lines to write for a sync track.
        /// </summary>
        /// <returns>Enumerable of all the lines</returns>
        private static IEnumerable<string> GetSyncTrackLines(SyncTrack syncTrack)
        {
            if (syncTrack is null)
                yield break;

            //Loop through time signatures and tempo markers, picked using the lowest position
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
        private static string GetWrittenFloat(float value) => ((int)(value * 1000)).ToString().Replace(".", "").Replace(",", "");
        /// <summary>
        /// Gets the lines to write for a part.
        /// </summary>
        /// <returns>Enumerable of all the lines</returns>
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
        private static string GetLine(string header, string value) => $"  {header} = {value}";
        /// <summary>
        /// Gets the written data for a note.
        /// </summary>
        internal static string GetNoteData(byte index, uint sustain) => $"N {index} {sustain}";
    }
}
