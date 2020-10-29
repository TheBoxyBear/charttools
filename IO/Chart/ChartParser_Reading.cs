using ChartTools.Lyrics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChartTools.IO.Chart
{
    /// <summary>
    /// Provides methods for reading chart files
    /// </summary>
    internal static partial class ChartParser
    {
        /// <summary>
        /// Part names of standard instruments
        /// </summary>
        private static readonly Dictionary<StandardInstrument, string> StandardInstrumentsDictionary = new Dictionary<StandardInstrument, string>()
        {
            { StandardInstrument.LeadGuitar, "Single" },
            { StandardInstrument.RhythmGuitar, "DoubleRhythm" },
            { StandardInstrument.CoopGuitar, "DoubleGuitar" },
            { StandardInstrument.Bass, "DoubleBass" },
            { StandardInstrument.Keys, "Keyboard" }
        };

        /// <summary>
        /// Gets the part name for drums.
        /// </summary>
        private static string GetDrumsPartName() => $"Drums";
        /// <summary>
        /// Gets the part name for a standard instrument.
        /// </summary>
        private static string GetPartName(StandardInstrument instrument) => $"{StandardInstrumentsDictionary[instrument]}";
        /// <summary>
        /// Gets the part name for a Guitar Hero Live instrument.
        /// </summary>
        private static string GetPartName(GHLInstrument instrument) => $"GHL{instrument}";

        /// <summary>
        /// Reads a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Song"/> contianing all song data
        ///     <para><see langword="null"/> if the file contains no song data</para>
        /// </returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        internal static Song ReadSong(string path)
        {
            IEnumerable<string> lines;

            try { lines = GetLines(path); }
            catch (Exception e) { throw e; }

            Song song = new Song();

            List<Task> tasks = new List<Task>()
            {
                Task.Run(() =>
                {
                    try {song.Metadata = GetMetadata(lines); }
                    catch (Exception e) { throw e; }
                }),
                Task.Run(() =>
                {
                    try { song.GlobalEvents = GetGlobalEvents(lines).ToList(); }
                    catch (Exception e) { throw e; }
                }),
                Task.Run(() =>
                {
                    try { song.SyncTrack = GetSyncTrack(lines); }
                    catch (Exception e) { throw e; }
                }),
                Task.Run(() =>
                {
                    try {song.Drums = GetInstrument(lines, part => GetDrumsTrack(part), GetDrumsPartName()); }
                    catch (Exception e) { throw e; }
                })
            };

            foreach (GHLInstrument instrument in EnumExtensions.GetValues<GHLInstrument>())
                tasks.Add(Task.Run(() => song.GetType().GetProperty(GetPartName(instrument)).SetValue(song, GetInstrument(lines, part => GetGHLTrack(part), GetPartName(instrument)))));
            foreach (StandardInstrument instrument in EnumExtensions.GetValues<StandardInstrument>())
                tasks.Add(Task.Run(() =>
                    song.GetType().GetProperty(instrument.ToString()).SetValue(song, GetInstrument(lines, part => GetStandardTrack(part), GetPartName(instrument)))));

            foreach (Task task in tasks)
            {
                task.Wait();
                task.Dispose();
            }

            return song;
        }

        /// <summary>
        /// Reads drums from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="DrumsChord"/> containing all drums data
        ///     <para><see langword="null"/> if the file contains no drums data</para>
        /// </returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        internal static Instrument<DrumsChord> ReadDrums(string path) => GetInstrument(GetLines(path), part => GetDrumsTrack(part), GetDrumsPartName());
        /// <summary>
        /// Reads a Guitar Hero Live instrument from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="GHLChord"/> containing all data about the given instrument
        ///     <para><see langword="null"/> if the file has no data for the given instrument</para>
        /// </returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        internal static Instrument<GHLChord> ReadInstrument(string path, GHLInstrument instrument) => GetInstrument(GetLines(path), part => GetGHLTrack(part), GetPartName(instrument));
        /// <summary>
        /// Reads a standard instrument from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="StandardChord"/> containing all data about the given instrument
        ///     <para><see langword="null"/> if the file contains no data for the given instrument</para>
        /// </returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        internal static Instrument<StandardChord> ReadInstrument(string path, StandardInstrument instrument) => GetInstrument(GetLines(path), part => GetStandardTrack(part), GetPartName(instrument));
        /// <summary>
        /// Gets all data for an instrument from the contents of a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> containing all data about the given instrument
        ///     <para><see langword="null"/> if the file contains no data for the given instrument</para>
        /// </returns>
        /// <exception cref="FormatException"/>
        private static Instrument<TChord> GetInstrument<TChord>(IEnumerable<string> lines, Func<IEnumerable<string>, Track<TChord>> getTrack, string instrumentPartName) where TChord : Chord
        {
            Instrument<TChord> instrument = new Instrument<TChord>();
            List<Task> tasks = new List<Task>();

            foreach (Difficulty difficulty in EnumExtensions.GetValues<Difficulty>())
                tasks.Add(Task.Run(() =>
                {
                    Track<TChord> track;

                    try { track = getTrack(GetPart(lines, $"{difficulty}{instrumentPartName}")); }
                    catch (Exception e) { throw e; }

                    if (track is not null)
                        typeof(Instrument<TChord>).GetProperty(difficulty.ToString()).SetValue(instrument, track);
                }));

            Task.WaitAll(tasks.ToArray());

            foreach (Track<TChord> track in new Track<TChord>[]
            {
                instrument.Easy,
                instrument.Medium,
                instrument.Hard,
                instrument.Expert
            })
                if (track is not null)
                    return instrument;

            return null;
        }

        #region Tracks
        /// <summary>
        /// Reads a drums difficulty track from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChors is <see cref="DrumsChord"/> containing all drums data for the given difficulty
        ///     <para><see langword="null"/> if the file contians no drums data for the given difficulty</para>
        /// </returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        internal static Track<DrumsChord> ReadDrumsTrack(string path, Difficulty difficulty) => GetDrumsTrack(GetLines(path), difficulty);
        /// <summary>
        /// Gets a drums difficulty track from the contents of a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChord is <see cref="DrumsChord"/> containing all drums data for the given difficulty
        ///     <para><see langword="null"/> if the lines contain no drums data for the given difficulty</para>
        /// </returns>
        /// <exception cref="FormatException"/>
        private static Track<DrumsChord> GetDrumsTrack(IEnumerable<string> lines, Difficulty difficulty) => GetDrumsTrack(GetPart(lines, $"{difficulty}Drums"));
        /// <summary>
        /// Gets all data from a portion of a chart file containing a drums difficulty track.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChord is <see cref="DrumsChord"/> containing all data in the part
        ///     <para><see langword="null"/> if the part contains no data</para>
        /// </returns>
        /// <exception cref="FormatException"/>
        private static Track<DrumsChord> GetDrumsTrack(IEnumerable<string> part) => GetTrack<DrumsChord>(part, (track, chord, entry, data, newChord) =>
        {
            if (chord is null)
                chord = new DrumsChord(entry.Position);
            else if (entry.Position != chord.Position)
                chord = track.Chords.FirstOrDefault(c => c.Position == entry.Position, new DrumsChord(entry.Position), out newChord);
            else
                newChord = false;

            if (data.NoteIndex < 5)
                chord.Notes.Add(new DrumsNote((DrumsNotes)data.NoteIndex) { SustainLength = data.SustainLength });
            else if (data.NoteIndex > 66 && data.NoteIndex < 69)
            {
                DrumsNote note = null;
                byte seekedIndex = (byte)(data.NoteIndex - 63);
                bool returnedDefault = true;

                note = chord.Notes.FirstOrDefault(n => n.NoteIndex == seekedIndex, null, out returnedDefault);

                if (returnedDefault)
                    note.IsCymbal = true;
                else
                {
                    chord.Notes.Add(new DrumsNote((DrumsNotes)(seekedIndex + 1)) { IsCymbal = true, SustainLength = data.SustainLength });
                    returnedDefault = false;
                }
            }

            if (newChord)
                track.Chords.Add(chord);
            //Instance gets lost if not returned back to GetTrack
            return chord;
        });
        /// <summary>
        /// Reads a Guitar Hero Live difficulty track from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChors is <see cref="GHLChord"/> containing all data for the given instrument and difficulty
        ///     <para><see langword="null"/> if the file contians no data for the given instrument and difficulty</para>
        /// </returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        internal static Track<GHLChord> ReadTrack(string path, GHLInstrument instrument, Difficulty difficulty) => GetGHLTrack(GetLines(path), instrument, difficulty);
        /// <summary>
        /// Gets a Guitar Hero Live difficulty track from the contents of a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChord is <see cref="GHLChord"/> containing all data for the given instrument and difficulty
        ///     <para><see langword="null"/> if the lines contain no data for the given instrument and difficulty</para>
        /// </returns>
        /// <exception cref="FormatException"/>
        private static Track<GHLChord> GetGHLTrack(IEnumerable<string> lines, GHLInstrument instrument, Difficulty difficulty) => GetGHLTrack(GetPart(lines, $"{difficulty}{GetPartName(instrument)}"));
        /// <summary>
        /// Gets all data from a portion of a chart file containing a Guitar Hero Live difficulty track.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChord is <see cref="GHLChord"/> containing all data in the part
        ///     <para><see langword="null"/> if the part contains no data</para>
        /// </returns>
        /// <exception cref="FormatException"/>
        private static Track<GHLChord> GetGHLTrack(IEnumerable<string> part) => GetTrack<GHLChord>(part, (track, chord, entry, data, newChord) =>
        {
            if (chord is null)
                chord = new GHLChord(entry.Position);
            else if (entry.Position != chord.Position)
                chord = track.Chords.FirstOrDefault(c => c.Position == entry.Position, new GHLChord(entry.Position), out newChord);
            else
                newChord = false;

            if (data.NoteIndex < 3)
                chord.Notes.Add(new GHLNote((GHLNotes)(data.NoteIndex + 4)) { SustainLength = data.SustainLength });
            else if (data.NoteIndex < 5)
                chord.Notes.Add(new GHLNote((GHLNotes)(data.NoteIndex - 2)) { SustainLength = data.SustainLength });
            else
                switch (data.NoteIndex)
                {
                    case 5:
                        chord.Modifier |= GHLChordModifier.Forced;
                        break;
                    case 6:
                        chord.Modifier |= GHLChordModifier.Tap; ;
                        break;
                    case 7:
                        chord.Notes.Add(new GHLNote(GHLNotes.Open) { SustainLength = data.SustainLength });
                        break;
                    case 8:
                        chord.Notes.Add(new GHLNote(GHLNotes.Black3) { SustainLength = data.SustainLength });
                        break;
                }

            if (newChord)
                track.Chords.Add(chord);
            //Instance gets lost if not returned back to GetTrack
            return chord;
        });
        /// <summary>
        /// Reads a standard difficulty track from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChors is <see cref="StandardChord"/> containing all drums data for the given instrument and difficulty
        ///     <para><see langword="null"/> if the file contians no data for the given instrument and difficulty</para>
        /// </returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        internal static Track<StandardChord> ReadTrack(string path, StandardInstrument instrument, Difficulty difficulty) => GetStandardTrack(GetLines(path), instrument, difficulty);
        /// <summary>
        /// Gets a standard difficulty track from the contents of a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChord is <see cref="StandardChord"/> containing all data for the given instrument and difficulty
        ///     <para><see langword="null"/> if the lines contain no data for the given instrument and difficulty</para>
        /// </returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        private static Track<StandardChord> GetStandardTrack(IEnumerable<string> lines, StandardInstrument instrument, Difficulty difficulty) => GetStandardTrack(GetPart(lines, $"{difficulty}{GetPartName(instrument)}"));
        /// <summary>
        /// Gets all data from a portion of a chart file containing a standard difficulty track.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChord is <see cref="StandardChord"/> containing all data in the part
        ///     <para><see langword="null"/> if the part contains no data</para>
        /// </returns>
        /// <exception cref="FormatException"/>
        private static Track<StandardChord> GetStandardTrack(IEnumerable<string> part) => GetTrack<StandardChord>(part, (track, chord, entry, data, newChord) =>
        {
            if (chord is null)
                chord = new StandardChord(entry.Position);
            else if (entry.Position != chord.Position)
                chord = track.Chords.FirstOrDefault(c => c.Position == entry.Position, new StandardChord(entry.Position), out newChord);
            else
                newChord = false;

            if (data.NoteIndex < 5)
                chord.Notes.Add(new StandardNote((StandardNotes)(data.NoteIndex + 1)) { SustainLength = data.SustainLength });
            else
                switch (data.NoteIndex)
                {
                    case 5:
                        chord.Modifier |= StandardChordModifier.Forced;
                        break;
                    case 6:
                        chord.Modifier |= StandardChordModifier.Tap;
                        break;
                    case 7:
                        chord.Notes.Add(new StandardNote(StandardNotes.Open) { SustainLength = data.SustainLength });
                        break;
                }

            if (newChord)
                track.Chords.Add(chord);

            //Instance gets lost if not returned back to GetTrack
            return chord;
        });
        /// <summary>
        /// Gets a difficulty track from a portion of a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChord containing all data in the part
        ///     <para><see langword="null"/> if the part contains no data</para>
        /// </returns>
        /// <exception cref="FormatException"/>
        private static Track<TChord> GetTrack<TChord>(IEnumerable<string> part, Func<Track<TChord>, TChord, TrackObjectEntry, NoteData, bool, TChord> noteCase) where TChord : Chord
        {
            Track<TChord> track = new Track<TChord>();
            TChord chord = null;
            bool newChord = true;

            foreach (string line in part)
            {
                TrackObjectEntry entry;

                try { entry = new TrackObjectEntry(line); }
                catch (Exception e) { throw GetException(line, e); }

                switch (entry.Type)
                {
                    case "E":
                        string[] split = GetDataSplit(entry.Data);

                        track.LocalEvents.Add(new LocalEvent(entry.Position, split[0], split.Length == 1 ? "" : split[1]));
                        break;
                    case "N":
                        NoteData data;
                        try
                        {
                            data = new NoteData(entry.Data); 
                            chord = noteCase(track, chord, entry, data, newChord);
                        }
                        catch (Exception e) { throw GetException(line, e); }

                        break;
                    case "S":
                        try
                        {
                            split = GetDataSplit(entry.Data);
                            uint length;

                            if (!uint.TryParse(split[1], out length))
                                throw new FormatException($"Cannot parse length \"{split[0]}\" to uint.");

                            track.StarPowerPhrases.Add(new StarPowerPhrase(entry.Position, length));
                        }
                        catch (Exception e) { throw GetException(line, e); }
                        break;
                }
            }

            byte emptyCount = 0;

            if (track.Chords.Count == 0)
            {
                emptyCount++;
                track.Chords = null;
            }
            if (track.LocalEvents.Count == 0)
            {
                emptyCount++;
                track.LocalEvents = null;
            }
            if (track.StarPowerPhrases.Count == 0)
            {
                emptyCount++;
                track.StarPowerPhrases = null;
            }

            return emptyCount == 3 ? null : track;
        }
        #endregion

        /// <summary>
        /// Splits the data of an entry.
        /// </summary>
        private static string[] GetDataSplit(string data) => data.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        /// <summary>
        /// Generates an exception to throw when a line cannot be converted.
        /// </summary>
        /// <returns>Instance of <see cref="Exception"/> to throw</returns>
        private static Exception GetException(string line, Exception innerException) => new FormatException($"Line \"{line}\": {innerException.Message}", innerException);

        /// <summary>
        /// Reads the metadata from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Metadata"/> containing metadata from the file
        ///     <para>Null if the file contains no metadata</para>
        /// </returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        internal static Metadata ReadMetadata(string path) => GetMetadata(GetLines(path));
        /// <summary>
        /// Gets the metadata from the contents of a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Metadata"/> containing metadata from the file
        ///     <para>Null if the lines contain no metadata</para>
        /// </returns>
        /// <exception cref="FormatException"
        private static Metadata GetMetadata(IEnumerable<string> lines)
        {
            Metadata metadata = new Metadata();

            foreach (string line in GetPart(lines, "Song"))
            {
                ChartEntry entry;
                try { entry = new ChartEntry(line); }
                catch (Exception e) { throw GetException(line, e); }

                string data = entry.Data.Trim('"');

                switch (entry.Header)
                {
                    case "Name":
                        metadata.Title = data;
                        break;
                    case "Artist":
                        metadata.Artist = data;
                        break;
                    case "Charter":
                        metadata.Charter = new Charter() { Name = data };
                        break;
                    case "Album":
                        metadata.Album = data;
                        break;
                    case "Year":
                        ushort year;
                        try { year = ushort.Parse(data.TrimStart(',')); }
                        catch (Exception e) { throw GetException(line, e); }
                        metadata.Year = year;
                        break;
                    case "Offset":
                        float offset;
                        try { offset = float.Parse(entry.Data); }
                        catch (Exception e) { throw GetException(line, e); }
                        metadata.AudioOffset = offset;
                        break;
                    case "Resolution":
                        ushort resolution;
                        try { resolution = ushort.Parse(data); }
                        catch (Exception e) { throw GetException(line, e); }
                        metadata.Resolution = resolution;
                        break;
                    case "Difficulty":
                        sbyte difficulty;
                        try { difficulty = sbyte.Parse(data); }
                        catch (Exception e) { throw GetException(line, e); }
                        metadata.Difficulty = difficulty;
                        break;
                    case "PreviewStart":
                        ushort previewStart;
                        try { previewStart = ushort.Parse(data); }
                        catch (Exception e) { throw GetException(line, e); }
                        metadata.PreviewStart = previewStart;
                        break;
                    case "PreviewEnd":
                        ushort previewEnd;
                        try { previewEnd = ushort.Parse(data); }
                        catch (Exception e) { throw GetException(line, e); }
                        metadata.PreviewEnd = previewEnd;
                        break;
                    case "Genre":
                        metadata.Genre = data;
                        break;
                    case "MediaType":
                        metadata.MediaType = data;
                        break;
                    case "MusicStream":
                        metadata.Streams.Music = data;
                        break;
                    case "GuitarStream":
                        metadata.Streams.Guitar = data;
                        break;
                    case "BassStream":
                        metadata.Streams.Bass = data;
                        break;
                    case "RhythmStream":
                        if (metadata.Streams is null)
                            metadata.Streams = new StreamCollection();
                        metadata.Streams.Rhythm = data;
                        break;
                    case "KeysStream":
                        if (metadata.Streams is null)
                            metadata.Streams = new StreamCollection();
                        metadata.Streams.Keys = data;
                        break;
                    case "DrumStream":
                        if (metadata.Streams is null)
                            metadata.Streams = new StreamCollection();
                        metadata.Streams.Drum = data;
                        break;
                    case "Drum2Stream":
                        if (metadata.Streams is null)
                            metadata.Streams = new StreamCollection();
                        metadata.Streams.Drum2 = data;
                        break;
                    case "Drum3Stream":
                        if (metadata.Streams is null)
                            metadata.Streams = new StreamCollection();
                        metadata.Streams.Drum3 = data;
                        break;
                    case "Drum4Stream":
                        if (metadata.Streams is null)
                            metadata.Streams = new StreamCollection();
                        metadata.Streams.Drum4 = data;
                        break;
                    case "VocalStream":
                        if (metadata.Streams is null)
                            metadata.Streams = new StreamCollection();
                        metadata.Streams.Vocal = data;
                        break;
                    case "CrowdStream":
                        if (metadata.Streams is null)
                            metadata.Streams = new StreamCollection();
                        metadata.Streams.Crowd = data;
                        break;
                }
            }

            return metadata;
        }

        /// <summary>
        /// Reads the lyrics from a chart file.
        /// </summary>
        /// <returns>
        /// Enumerable of <see cref="Phrase"/> containing the lyrics from the file</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        internal static IEnumerable<Phrase> ReadLyrics(string path)
        {
            Phrase newPhrase = null;
            IEnumerable<GlobalEvent> events;

            try { events = ReadGlobalEvents(path); }
            catch (Exception e) { throw e; }

            foreach (GlobalEvent e in events.OrderBy(e => e.Position))
                switch (e.EventType)
                {
                    case GlobalEventType.PhraseStart:
                        if (e.EventType == GlobalEventType.PhraseStart)
                        {
                            if (newPhrase is not null)
                                yield return newPhrase;

                            newPhrase = new Phrase(e.Position);
                        }
                        break;
                    case GlobalEventType.Lyric:
                        if (newPhrase is null)
                            newPhrase = new Phrase(e.Position);

                        newPhrase.Syllables.Add(new Syllable(e.Position) { RawText = e.Argument });
                        break;
                    case GlobalEventType.PhraseEnd:
                        if (newPhrase is not null)
                            newPhrase.EndPosition = e.Position;
                        yield return newPhrase;
                        break;
                }
        }

        /// <summary>
        /// Reads the global events from a chart file.
        /// </summary>
        /// <returns>Enumerable of <see cref="GlobalEvent"/></returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        internal static IEnumerable<GlobalEvent> ReadGlobalEvents(string path) => GetGlobalEvents(GetLines(path));
        /// <summary>
        /// Gets the global events from the contents of a chart file.
        /// </summary>
        /// <returns>Enumerable of <see cref="GlobalEvent"/></returns>
        /// <exception cref="FormatException"/>
        private static IEnumerable<GlobalEvent> GetGlobalEvents(IEnumerable<string> lines)
        {
            foreach (string line in GetPart(lines, "Events"))
            {
                TrackObjectEntry entry;
                try { entry = new TrackObjectEntry(line); }
                catch (Exception e) { throw GetException(line, e); }

                string[] split = GetDataSplit(entry.Data.Trim('"'));
                yield return new GlobalEvent(entry.Position, split[0], split.Length == 1 ? "" : split[1]);
            }
        }

        /// <summary>
        /// Reads the sync track from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="SyncTrack"/>
        ///     <para><see langword="null"/> if the file contains no sync track</para>
        /// </returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        internal static SyncTrack ReadSyncTrack(string path) => GetSyncTrack(GetLines(path));
        /// <summary>
        /// Gets the sync track from the contents of a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="SyncTrack"/>
        ///     <para><see langword="null"/> if the lines contain no sync track</para>
        /// </returns> 
        /// <exception cref="FormatException"/>
        private static SyncTrack GetSyncTrack(IEnumerable<string> lines)
        {
            SyncTrack syncTrack = new SyncTrack();

            foreach (string line in GetPart(lines, "SyncTrack"))
            {
                TrackObjectEntry entry;
                try { entry = new TrackObjectEntry(line); }
                catch (Exception e) { throw GetException(line, e); }

                Tempo marker;

                switch (entry.Type)
                {
                    case "TS":
                        string[] split = GetDataSplit(entry.Data);

                        byte numerator, denominator;

                        if (!byte.TryParse(split[0], out numerator))
                            throw new FormatException($"Cannot parse numerator \"{split[0]}\" to byte.");

                        if (split.Length < 2)
                            denominator = 4;
                        else
                        {
                            if (byte.TryParse(split[1], out denominator))
                                denominator = (byte)Math.Pow(2, denominator);
                            else
                                throw new FormatException($"Cannot parse denominator \"{split[1]}\" to byte.");
                        }

                        try { syncTrack.TimeSignatures.Add(new TimeSignature(entry.Position, numerator, denominator)); }
                        catch (Exception e) { throw e; }
                        break;
                    case "B":
                        float value;

                        if (float.TryParse(entry.Data, out value))
                            value /= 1000;
                        else
                            throw new FormatException($"Cannot parse value \"{entry.Data}\" to float.");

                        marker = syncTrack.Tempo.FirstOrDefault(m => m.Position == entry.Position);

                        if (marker is null)
                            try { syncTrack.Tempo.Add(new Tempo(entry.Position, value)); }
                            catch (Exception e) { throw e; }
                        else
                            marker.Value = value;
                        break;
                    case "A":
                        float anchor;

                        if (float.TryParse(entry.Data, out anchor))
                            anchor /= 1000;
                        else
                            throw new FormatException($"Cannot parse value \"{entry.Data}\" to float.");

                        marker = syncTrack.Tempo.FirstOrDefault(m => m.Position == entry.Position);

                        if (marker is null)
                            try { syncTrack.Tempo.Add(new Tempo(entry.Position, 0) { Anchor = anchor }); }
                            catch (Exception e) { throw e; }
                        else
                            marker.Anchor = anchor;

                        break;
                }
            }

            return syncTrack.TimeSignatures.Count == 0 && syncTrack.Tempo.Count == 0 ? null : syncTrack;
        }

        /// <summary>
        /// Gets the lines from a text file.
        /// </summary>
        /// <returns>Enumerable of all the lines in the file</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        private static IEnumerable<string> GetLines(string path)
        {
            StreamReader reader;

            try { reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)); }
            catch (Exception e) { throw e; }

            using (reader)
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (line != string.Empty)
                        yield return line;
                }
        }
        /// <summary>
        /// Gets a part from the contents of a chart file
        /// </summary>
        /// <returns>Enumesable of all the lines in the part</returns>
        /// <exception cref="InvalidDataException"/>
        private static IEnumerable<string> GetPart(IEnumerable<string> lines, string partName)
        {
            IEnumerator<string> enumerator = lines.GetEnumerator();

            enumerator.MoveNext();

            while (enumerator.Current != $"[{partName}]")
                if (!enumerator.MoveNext())
                    yield break;

            //Move down two lines
            for (int i = 0; i < 2; i++)
                if (!enumerator.MoveNext())
                    yield break;

            while (enumerator.Current != "}")
            {
                yield return enumerator.Current;

                if (!enumerator.MoveNext())
                    throw new InvalidDataException($"Part \"{partName}\" did not end within the provided lines.");
            }
        }
        /// <summary>
        /// Reads a part from a chart file.
        /// </summary>
        /// <returns>Enumerable of all the lines in the part</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="InvalidDataException"/>
        /// <exception cref="OutOfMemoryException"/>
        private static IEnumerable<string> GetPart(string path, string partName) => GetPart(GetLines(path), partName);
    }
}
