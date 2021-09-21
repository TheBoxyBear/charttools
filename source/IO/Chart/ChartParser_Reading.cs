using ChartTools.Lyrics;
using ChartTools.SystemExtensions.Linq;

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
        internal static readonly ReadingConfiguration DefaultReadConfig = new() { SoloNoStarPowerRule = SoloNoStarPowerPolicy.Convert };
        private delegate void NoteCase<TChord>(Track<TChord> track, ref TChord? chord, uint position, NoteData data, ref bool newChord) where TChord : Chord;

        /// <summary>
        /// Reads a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Song"/> containing all song data</returns>
        /// <param name="path">Path of the file to read</param>
        /// <inheritdoc cref="ReadFile(string)" path="/exception"/>
        public static Song ReadSong(string path, ReadingConfiguration? config)
        {
            string[] lines = ReadFile(path).ToArray();

            Song song = new();
            Type songType = typeof(Song);

            // Add threads to read metadata, global events, sync track and drums
            List<Task> tasks = new()
            {
                Task.Run(() => song.Metadata = GetMetadata(lines)),
                Task.Run(() => song.GlobalEvents = GetGlobalEvents(lines, config).ToList()),
                Task.Run(() => song.SyncTrack = GetSyncTrack(lines, config)),
                Task.Run(() => song.Drums = GetInstrument(lines, part => GetDrumsTrack(part, config), partNames[Instruments.Drums]))
            };
            // Add a thread to read each GHL instrument
            foreach (GHLInstrument instrument in Enum.GetValues<GHLInstrument>())
                tasks.Add(Task.Run(() => songType.GetProperty($"GHL{instrument}")!.SetValue(song, GetInstrument(lines, part => GetGHLTrack(part, config), partNames[(Instruments)instrument]))));
            // Add a thread to read each standard instrument
            foreach (StandardInstrument instrument in Enum.GetValues<StandardInstrument>())
                tasks.Add(Task.Run(() => songType.GetProperty(instrument.ToString())!.SetValue(song, GetInstrument(lines, part => GetStandardTrack(part, config), partNames[(Instruments)instrument]))));

            Task.WaitAll(tasks.ToArray());

            return song;
        }

        #region Instruments
        /// <summary>
        /// Reads an instrument from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument"/> containing all data about the given instrument
        ///     <para><see langword="null"/> if the file contains no data for the given instrument</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <param name="instrument">Instrument to read</param>
        /// <inheritdoc cref="ReadDrums(string, ReadingConfiguration)" path="/exception"/>
        /// <inheritdoc cref="ReadInstrument(string, GHLInstrument, ReadingConfiguration)" path="/exception"/>
        /// <inheritdoc cref="ReadInstrument(string, StandardInstrument, ReadingConfiguration)" path="/exception"/>
        public static Instrument? ReadInstrument(string path, Instruments instrument, ReadingConfiguration config)
        {
            if (instrument == Instruments.Drums)
                return ReadDrums(path, config);
            if (Enum.IsDefined((GHLInstrument)instrument))
                return ReadInstrument(path, (GHLInstrument)instrument, config);
            if (Enum.IsDefined((StandardInstrument)instrument))
                return ReadInstrument(path, (StandardInstrument)instrument, config);

            throw CommonExceptions.GetUndefinedException(instrument);
        }

        /// <summary>
        /// Reads drums from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="DrumsChord"/> containing all drums data
        ///     <para><see langword="null"/> if the file contains no drums data</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <inheritdoc cref="ReadFile(string)" path="/exception"/>
        /// <inheritdoc cref="GetDrumsTrack(IEnumerable{string}, ReadingConfiguration)(IEnumerable{string}, ReadingConfiguration)" path="/exception"/>
        public static Instrument<DrumsChord>? ReadDrums(string path, ReadingConfiguration config) => GetInstrument(ReadFile(path).ToArray(), part => GetDrumsTrack(part, config), partNames[Instruments.Drums]);
        /// <summary>
        /// Reads a Guitar Hero Live instrument from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="GHLChord"/> containing all data about the given instrument
        ///     <para><see langword="null"/> if the file has no data for the given instrument</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <inheritdoc cref="ReadFile(string)" path="/exception"/>
        /// <inheritdoc cref="GetGHLTrack(IEnumerable{string}, ReadingConfiguration)(IEnumerable{string}, ReadingConfiguration)" path="/exception"/>
        public static Instrument<GHLChord>? ReadInstrument(string path, GHLInstrument instrument, ReadingConfiguration config) => GetInstrument(ReadFile(path).ToArray(), part => GetGHLTrack(part, config), partNames[(Instruments)instrument]);
        /// <summary>
        /// Reads a standard instrument from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="StandardChord"/> containing all data about the given instrument
        ///     <para><see langword="null"/> if the file contains no data for the given instrument</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <param name="instrument">Instrument to read</param>
        /// <inheritdoc cref="ReadFile(string)" path="/exception"/>
        /// <inheritdoc cref="GetStandardTrack(IEnumerable{string}, ReadingConfiguration)" path="/exception"/>
        public static Instrument<StandardChord>? ReadInstrument(string path, StandardInstrument instrument, ReadingConfiguration config) => GetInstrument(ReadFile(path).ToArray(), part => GetStandardTrack(part, config), partNames[(Instruments)instrument]);
        /// <summary>
        /// Gets all data for an instrument from the contents of a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> containing all data about the given instrument
        ///     <para><see langword="null"/> if the file contains no data for the given instrument</para>
        /// </returns>
        /// <param name="lines">Lines in the file</param>
        /// <param name="getTrack">Function that retrieves the track from the lines</param>
        /// <param name="instrumentPartName">Part name of the instrument excluding the difficulty</param>
        private static Instrument<TChord>? GetInstrument<TChord>(string[] lines, Func<IEnumerable<string>, Track<TChord>?> getTrack, string instrumentPartName) where TChord : Chord
        {
            Instrument<TChord> instrument = new();
            // Difficulties are read in reverse order to give harder, more complex ones a head-start while tasks are still being fired
            Difficulty[] difficulties = Enum.GetValues<Difficulty>().Reverse().ToArray();
            Task<Track<TChord>?>[] tasks = difficulties.Select(d => Task.Run(() => getTrack(GetPart(lines, $"{d}{instrumentPartName}")))).ToArray();

            Task.WaitAll(tasks);

            if (tasks.Any(t => t.Result is not null))
            {
                if (tasks.Any(t => t.Result is not null))
                {
                    Type instrumentType = typeof(Instrument<TChord>);

                    for (int i = 0; i < difficulties.Length; i++)
                        if (tasks[i].Result is not null)
                            instrumentType.GetProperty(difficulties[i].ToString())!.SetValue(instrument, tasks[i].Result);
                }

                return instrument;
            }

            return instrument.Difficulty is null
                && difficulties.Select(d => instrument.GetTrack(d)).All(t => t is null)
                ? null : instrument;
        }
        #endregion
        #region Tracks
        /// <summary>
        /// Reads a track from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track"/> containing all data about the given track
        ///     <para><see langword="null"/> if the file contains no data for the given track</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <param name="instrument">Instrument of the track to read</param>
        /// <param name="difficulty">Difficulty of the track to read</param>
        /// <inheritdoc cref="ReadDrumsTrack(string, Difficulty, ReadingConfiguration)" path="/exception"/>
        /// <inheritdoc cref="ReadTrack(string, GHLInstrument, Difficulty, ReadingConfiguration)" path="/exception"/>
        /// <inheritdoc cref="ReadTrack(string, StandardInstrument, Difficulty, ReadingConfiguration)" path="/exception"/>
        public static Track? ReadTrack(string path, Instruments instrument, Difficulty difficulty, ReadingConfiguration config)
        {
            if (instrument == Instruments.Drums)
                return ReadDrumsTrack(path, difficulty, config);
            if (Enum.IsDefined((GHLInstrument)instrument))
                return ReadTrack(path, (GHLInstrument)instrument, difficulty, config);
            if (Enum.IsDefined((StandardInstrument)instrument))
                return ReadTrack(path, (StandardInstrument)instrument, difficulty, config);

            throw CommonExceptions.GetUndefinedException(instrument);
        }

        /// <summary>
        /// Reads a drums track from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChors is <see cref="DrumsChord"/> containing all drums data for the given difficulty
        ///     <para><see langword="null"/> if the file contains no drums data for the given difficulty</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <param name="difficulty">Difficulty of the track to read</param>
        /// <inheritdoc cref="GetDrumsTrack(IEnumerable{string}, Difficulty, ReadingConfiguration), GHLInstrument, Difficulty, ReadingConfiguration)" path="/exception"/>
        /// <inheritdoc cref="GetPart(IEnumerable{string}, string)" path="/exception"/>
        /// <inheritdoc cref="GetFullPartName(Instruments, Difficulty)(IEnumerable{string}, string)" path="/exception"/>
        public static Track<DrumsChord>? ReadDrumsTrack(string path, Difficulty difficulty, ReadingConfiguration? config) => GetDrumsTrack(ReadFile(path), difficulty, config);
        /// <summary>
        /// Gets a drums track from the contents of a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChord is <see cref="DrumsChord"/> containing all drums data for the given difficulty
        ///     <para><see langword="null"/> if the lines contain no drums data for the given difficulty</para>
        /// </returns>
        /// <param name="lines">Lines in the file</param>
        /// <param name="difficulty">Difficulty of the track</param>
        /// <inheritdoc cref="GetDrumsTrack(IEnumerable{string}, ReadingConfiguration)" path="/exception"/>
        /// <inheritdoc cref="GetPart(IEnumerable{string}, string)" path="/exception"/>
        /// <inheritdoc cref="GetFullPartName(Instruments, Difficulty)(IEnumerable{string}, string)" path="/exception"/>
        public static Track<DrumsChord>? GetDrumsTrack(IEnumerable<string> lines, Difficulty difficulty, ReadingConfiguration? config) => GetDrumsTrack(GetPart(lines, GetFullPartName(Instruments.Drums, difficulty)), config);
        /// <summary>
        /// Gets all data from a portion of a chart file containing a drums track.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChord is <see cref="DrumsChord"/> containing all data in the part
        ///     <para><see langword="null"/> if the part contains no data</para>
        /// </returns>
        /// <param name="part">Lines of the file belonging to the track</param>
        /// <inheritdoc cref="GetTrack{TChord}(IEnumerable{string}, Func{Track{TChord}, TChord, TrackObjectEntry, NoteData, bool, TChord}, ReadingConfiguration)" path="/exception"/>
        public static Track<DrumsChord>? GetDrumsTrack(IEnumerable<string> part, ReadingConfiguration? config) => GetTrack<DrumsChord>(part, DrumsNoteCase, config);

        /// <summary>
        /// Reads a Guitar Hero Live track from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChors is <see cref="GHLChord"/> containing all data for the given instrument and difficulty
        ///     <para><see langword="null"/> if the file contains no data for the given instrument and difficulty</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <param name="instrument">Instrument of the track</param>
        /// <param name="difficulty">Difficulty of the track</param>
        /// <inheritdoc cref="GetGHLTrack(IEnumerable{string}, GHLInstrument, Difficulty, ReadingConfiguration)" path="/exception"/>
        /// <inheritdoc cref="GetPart(IEnumerable{string}, string)" path="/exception"/>
        /// <inheritdoc cref="GetFullPartName(Instruments, Difficulty)(IEnumerable{string}, string)" path="/exception"/>
        public static Track<GHLChord>? ReadTrack(string path, GHLInstrument instrument, Difficulty difficulty, ReadingConfiguration? config) => GetGHLTrack(ReadFile(path), instrument, difficulty, config);
        /// <summary>
        /// Gets a Guitar Hero Live track from the contents of a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChord is <see cref="GHLChord"/> containing all data for the given instrument and difficulty
        ///     <para><see langword="null"/> if the lines contain no data for the given instrument and difficulty</para>
        /// </returns>
        /// <param name="lines">Lines in the file</param>
        /// <param name="instrument">Instrument of the track</param>
        /// <param name="difficulty">Difficulty of the track</param>
        /// <inheritdoc cref="GetGHLTrack(IEnumerable{string}, GHLInstrument, Difficulty, ReadingConfiguration)" path="/exception"/>
        /// <inheritdoc cref="GetPart(IEnumerable{string}, string)" path="/exception"/>
        /// <inheritdoc cref="GetFullPartName(Instruments, Difficulty)(IEnumerable{string}, string)" path="/exception"/>
        private static Track<GHLChord>? GetGHLTrack(IEnumerable<string> lines, GHLInstrument instrument, Difficulty difficulty, ReadingConfiguration? config) => GetGHLTrack(GetPart(lines, GetFullPartName((Instruments)instrument, difficulty)), config);
        /// <summary>
        /// Gets all data from a portion of a chart file containing a Guitar Hero Live track.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChord is <see cref="GHLChord"/> containing all data in the part
        ///     <para><see langword="null"/> if the part contains no data</para>
        /// </returns>
        /// <param name="part">Lines in the file belonging to the track</param>
        /// <inheritdoc cref="GetTrack{TChord}(IEnumerable{string}, Func{Track{TChord}, TChord, TrackObjectEntry, NoteData, bool, TChord}, ReadingConfiguration)" path="/exception"/>
        private static Track<GHLChord>? GetGHLTrack(IEnumerable<string> part, ReadingConfiguration? config) => GetTrack<GHLChord>(part, GHLNoteCase, config);

        /// <summary>
        /// Reads a standard track from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChors is <see cref="StandardChord"/> containing all drums data for the given instrument and difficulty
        ///     <para><see langword="null"/> if the file contains no data for the given instrument and difficulty</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <param name="instrument">Instrument of the track</param>
        /// <param name="difficulty">Difficulty of the track</param>
        /// <inheritdoc cref="GetStandardTrack(IEnumerable{string}, StandardInstrument, Difficulty, ReadingConfiguration)"/>
        /// <inheritdoc cref="ReadFile(string)" path="/exception"/>
        public static Track<StandardChord>? ReadTrack(string path, StandardInstrument instrument, Difficulty difficulty, ReadingConfiguration config) => GetStandardTrack(ReadFile(path), instrument, difficulty, config);
        /// <summary>
        /// Gets a standard track from the contents of a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChord is <see cref="StandardChord"/> containing all data for the given instrument and difficulty
        ///     <para><see langword="null"/> if the lines contain no data for the given instrument and difficulty</para>
        /// </returns>
        /// <param name="lines">Liens in the file</param>
        /// <param name="instrument">Instrument of the track</param>
        /// <param name="difficulty">Difficulty of the track</param>
        /// <inheritdoc cref="GetTrack{TChord}(IEnumerable{string}, Func{Track{TChord}, TChord, TrackObjectEntry, NoteData, bool, TChord}, ReadingConfiguration)" path="/exception"/>
        /// <inheritdoc cref="GetPart(IEnumerable{string}, string)" path="/exception"/>
        /// <inheritdoc cref="GetFullPartName(Instruments, Difficulty)(IEnumerable{string}, string)" path="/exception"/>
        private static Track<StandardChord>? GetStandardTrack(IEnumerable<string> lines, StandardInstrument instrument, Difficulty difficulty, ReadingConfiguration config) => GetStandardTrack(GetPart(lines, GetFullPartName((Instruments)instrument, difficulty)), config);
        /// <summary>
        /// Gets all data from a portion of a chart file containing a standard track.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChord is <see cref="StandardChord"/> containing all data in the part
        ///     <para><see langword="null"/> if the part contains no data</para>
        /// </returns>
        /// <param name="part">Lines in the file belonging to the track</param>
        /// <inheritdoc cref="GetTrack{TChord}(IEnumerable{string}, Func{Track{TChord}, TChord, TrackObjectEntry, NoteData, bool, TChord}, ReadingConfiguration)" path="/exception"/>
        private static Track<StandardChord>? GetStandardTrack(IEnumerable<string> part, ReadingConfiguration? config) => GetTrack<StandardChord>(part, StandardNoteCase, config);

        /// <summary>
        /// Gets a track from a portion of a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChord containing all data in the part
        ///     <para><see langword="null"/> if the part contains no data</para>
        /// </returns>
        /// <param name="part">Lines in the file belonging to the track</param>
        /// <param name="noteCase">Function that handles entries containing note data. Must return the same chord received as a parameter.</param>
        /// <exception cref="FormatException"/>
        private static Track<TChord>? GetTrack<TChord>(IEnumerable<string> part, NoteCase<TChord> noteCase, ReadingConfiguration? config) where TChord : Chord
        {
            config ??= DefaultReadConfig;
            Track<TChord> track = new();
            TChord? chord = null;
            bool newChord = true;
            HashSet<(uint, byte)> ignoredNotes = new();
            Func<uint, NoteData, bool> includeNote = config.DuplicateTrackObjectPolicy switch
            {
                DuplicateTrackObjectPolicy.IncludeAll => (_, _) => true,
                DuplicateTrackObjectPolicy.IncludeFirst => (position, data) =>
                {
                    if (ignoredNotes.Contains((position, data.NoteIndex)))
                        return false;

                    ignoredNotes.Add((position, data.NoteIndex));
                    return true;
                },
                DuplicateTrackObjectPolicy.ThrowException => (position, data) =>
                {
                    if (ignoredNotes.Contains((position, data.NoteIndex)))
                        throw new Exception("Duplicate chord"); // TODO Make better exception
                    else
                    {
                        ignoredNotes.Add((position, data.NoteIndex));
                        return true;
                    }
                },
            };

            foreach (string line in part)
            {
                TrackObjectEntry entry;

                try { entry = new(line); }
                catch (Exception e) { throw GetLineException(line, e); }

                switch (entry.Type)
                {
                    // Local event
                    case "E":
                        string[] split = GetDataSplit(entry.Data.Trim('"'));
                        track.LocalEvents!.Add(new(entry.Position, split.Length > 0 ? split[0] : string.Empty));
                        break;
                    // Note or chord modifier
                    case "N":
                        NoteData data;
                        try
                        {
                            data = new(entry.Data);
                            noteCase(track, ref chord, entry.Position, data, ref newChord);

                            if (includeNote(entry.Position, data) && newChord)
                                track.Chords.Add(chord!);
                        }
                        catch (Exception e) { throw GetLineException(line, e); }

                        break;
                    // Star power
                    case "S":
                        try
                        {
                            split = GetDataSplit(entry.Data);

                            if (!uint.TryParse(split[1], out uint length))
                                throw new FormatException($"Cannot parse length \"{split[0]}\" to uint.");

                            track.StarPower.Add(new(entry.Position, length));
                        }
                        catch (Exception e) { throw GetLineException(line, e); }
                        break;
                }

                if (config.SoloNoStarPowerRule == SoloNoStarPowerPolicy.Convert)
                    track.StarPower.AddRange(track.SoloToStarPower(true));
            }

            // Return null if no data
            return track.Chords.Count == 0
                && track.LocalEvents!.Count == 0
                && track.StarPower.Count == 0
                ? null : track;
        }

        private static void DrumsNoteCase(Track<DrumsChord> track, ref DrumsChord? chord, uint position, NoteData data, ref bool newChord)
        {
            // Find the parent chord or create it
            if (chord is null)
                chord = new(position);
            else if (position != chord.Position)
                chord = track.Chords.FirstOrDefault(c => c.Position == position, new(position), out newChord);
            else
                newChord = false;

            switch (data.NoteIndex)
            {
                // Note
                case < 5:
                    chord!.Notes.Add(new((DrumsLane)data.NoteIndex) { SustainLength = data.SustainLength });
                    break;
                // Double kick
                case 32:
                    chord!.Notes.Add(new(DrumsLane.DoubleKick));
                    break;
                // Cymbal
                case > 65 and < 69:
                    DrumsNote? note = null;
                    // NoteIndex of the note to set as cymbal
                    byte seekedIndex = (byte)(data.NoteIndex - 64);

                    // Find matching note
                    note = chord!.Notes.FirstOrDefault(n => n.NoteIndex == seekedIndex, null, out bool returnedDefault);

                    if (returnedDefault)
                    {
                        chord.Notes.Add(new((DrumsLane)seekedIndex) { IsCymbal = true, SustainLength = data.SustainLength });
                        returnedDefault = false;
                    }
                    else
                        note!.IsCymbal = true;
                    break;
                case 109:
                    chord!.Modifier |= DrumsChordModifier.Flam;
                    break;
            }

        }
        private static void GHLNoteCase(Track<GHLChord> track, ref GHLChord? chord, uint position, NoteData data, ref bool newChord)
        {
            // Find the parent chord or create it
            if (chord is null)
                chord = new(position);
            else if (position != chord.Position)
                chord = track.Chords.FirstOrDefault(c => c.Position == position, new(position), out newChord);
            else
                newChord = false;

            switch (data.NoteIndex)
            {
                // White notes
                case < 3:
                    chord!.Notes.Add(new((GHLLane)(data.NoteIndex + 4)) { SustainLength = data.SustainLength });
                    break;
                // Black 1 and 2
                case < 5:
                    chord!.Notes.Add(new((GHLLane)(data.NoteIndex - 2)) { SustainLength = data.SustainLength });
                    break;
                case 5:
                    chord!.Modifier |= GHLChordModifier.Invert;
                    break;
                case 6:
                    chord!.Modifier |= GHLChordModifier.Tap;
                    break;
                case 7:
                    chord!.Notes.Add(new(GHLLane.Open) { SustainLength = data.SustainLength });
                    break;
                case 8:
                    chord!.Notes.Add(new(GHLLane.Black3) { SustainLength = data.SustainLength });
                    break;
            }
        }
        private static void StandardNoteCase(Track<StandardChord> track, ref StandardChord? chord, uint position, NoteData data, ref bool newChord)
        {
            // Find the parent chord or create it
            if (chord is null)
                chord = new(position);
            else if (position != chord.Position)
                chord = track.Chords.FirstOrDefault(c => c.Position == position, new(position), out newChord);
            else
                newChord = false;

            switch (data.NoteIndex)
            {
                // Colored note
                case < 5:
                    chord!.Notes.Add(new((StandardLane)(data.NoteIndex + 1)) { SustainLength = data.SustainLength });
                    break;
                case 5:
                    chord!.Modifier |= StandardChordModifier.Invert;
                    break;
                case 6:
                    chord!.Modifier |= StandardChordModifier.Tap;
                    break;
                case 7:
                    chord!.Notes.Add(new(StandardLane.Open) { SustainLength = data.SustainLength });
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Splits the data of an entry.
        /// </summary>
        /// <param name="data">Data portion of a <see cref="TrackObjectEntry"/></param>
        private static string[] GetDataSplit(string data) => data.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        /// <summary>
        /// Generates an exception to throw when a line cannot be converted.
        /// </summary>
        /// <returns>Instance of <see cref="Exception"/> to throw</returns>
        /// <param name="line">Line that caused the exception</param>
        /// <param name="innerException">Exception caught when interpreting the line</param>
        private static Exception GetLineException(string line, Exception innerException) => new FormatException($"Line \"{line}\": {innerException.Message}", innerException);

        /// <summary>
        /// Reads the metadata from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Metadata"/> containing metadata from the file
        ///     <para>Null if the file contains no metadata</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <inheritdoc cref="GetMetadata(string[])" path="/exception"/>
        /// <inheritdoc cref="ReadFile(string)" path="/exception"/>
        public static Metadata ReadMetadata(string path) => GetMetadata(ReadFile(path).ToArray());
        /// <summary>
        /// Gets the metadata from the contents of a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Metadata"/> containing metadata from the file
        ///     <para>Null if the lines contain no metadata</para>
        /// </returns>
        /// <param name="lines">Lines in the file</param>
        /// <inheritdoc cref="GetPart(IEnumerable{string}, string)" path="/exception"/>
        /// <exception cref="FormatException"/>
        private static Metadata GetMetadata(string[] lines)
        {
            Metadata metadata = new();

            foreach (string line in GetPart(lines, "Song"))
            {
                ChartEntry entry;
                try { entry = new(line); }
                catch (Exception e) { throw GetLineException(line, e); }

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
                        metadata.Charter = new() { Name = data };
                        break;
                    case "Album":
                        metadata.Album = data;
                        break;
                    case "Year":
                        try { metadata.Year = ushort.Parse(data.TrimStart(',')); }
                        catch (Exception e) { throw GetLineException(line, e); }
                        break;
                    case "Offset":
                        try { metadata.AudioOffset = int.Parse(entry.Data); }
                        catch (Exception e) { throw GetLineException(line, e); }
                        break;
                    case "Resolution":
                        try { metadata.Resolution = ushort.Parse(data); }
                        catch (Exception e) { throw GetLineException(line, e); }
                        break;
                    case "Difficulty":
                        try { metadata.Difficulty = sbyte.Parse(data); }
                        catch (Exception e) { throw GetLineException(line, e); }
                        break;
                    case "PreviewStart":
                        try { metadata.PreviewStart = uint.Parse(data); }
                        catch (Exception e) { throw GetLineException(line, e); }
                        break;
                    case "PreviewEnd":
                        try { metadata.PreviewEnd = uint.Parse(data); }
                        catch (Exception e) { throw GetLineException(line, e); }
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
                        metadata.Streams ??= new();
                        metadata.Streams.Rhythm = data;
                        break;
                    case "KeysStream":
                        metadata.Streams ??= new();
                        metadata.Streams.Keys = data;
                        break;
                    case "DrumStream":
                        metadata.Streams ??= new();
                        metadata.Streams.Drum = data;
                        break;
                    case "Drum2Stream":
                        metadata.Streams ??= new();
                        metadata.Streams.Drum2 = data;
                        break;
                    case "Drum3Stream":
                        metadata.Streams ??= new();
                        metadata.Streams.Drum3 = data;
                        break;
                    case "Drum4Stream":
                        metadata.Streams ??= new();
                        metadata.Streams.Drum4 = data;
                        break;
                    case "VocalStream":
                        metadata.Streams ??= new();
                        metadata.Streams.Vocal = data;
                        break;
                    case "CrowdStream":
                        metadata.Streams ??= new();
                        metadata.Streams.Crowd = data;
                        break;
                    default:
                        metadata.UnidentifiedData.Add(new() { Key = entry.Header, Data = entry.Data, Origin = FileFormat.Chart });
                        break;
                }
            }

            return metadata;
        }

        /// <summary>
        /// Reads the lyrics from a chart file.
        /// </summary>
        /// <returns>Enumerable of <see cref="Phrase"/> containing the lyrics from the file</returns>
        /// <param name="path">Path of the file to read</param>
        /// <inheritdoc cref="ReadGlobalEvents(string)(string[])" path="/exception"/>
        public static IEnumerable<Phrase> ReadLyrics(string path, ReadingConfiguration? config) => ReadGlobalEvents(path, config).GetLyrics();

        /// <summary>
        /// Reads the global events from a chart file.
        /// </summary>
        /// <returns>Enumerable of <see cref="GlobalEvent"/></returns>
        /// <param name="path">Path of the file the read</param>
        /// <inheritdoc cref="GetGlobalEvents(string[])" path="/exception"/>
        /// <inheritdoc cref="ReadFile(string)" path="/exception"/>
        public static IEnumerable<GlobalEvent> ReadGlobalEvents(string path, ReadingConfiguration? config) => GetGlobalEvents(ReadFile(path), config);
        /// <summary>
        /// Gets the global events from the contents of a chart file.
        /// </summary>
        /// <param name="lines">Lines in the file</param>
        /// <returns>Enumerable of <see cref="GlobalEvent"/></returns>
        /// <inheritdoc cref="GetPart(IEnumerable{string}, string)" path="/exception"/>
        /// <inheritdoc cref="TrackObjectEntry(string)" path="/exception"/>
        private static IEnumerable<GlobalEvent> GetGlobalEvents(IEnumerable<string> lines, ReadingConfiguration? config)
        {
            config ??= default;

            foreach (string line in GetPart(lines, "Events"))
            {
                TrackObjectEntry entry;
                try { entry = new(line); }
                catch (Exception e) { throw GetLineException(line, e); }

                GlobalEvent ev = new(entry.Position, entry.Data.Trim('"'));
                yield return ev;
            }
        }

        /// <summary>
        /// Reads the sync track from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="SyncTrack"/>
        ///     <para><see langword="null"/> if the file contains no sync track</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <inheritdoc cref="GetSyncTrack(string[])" path="/exception"/>
        /// <inheritdoc cref="ReadFile(string)" path="/exception"/>
        public static SyncTrack? ReadSyncTrack(string path, ReadingConfiguration? config) => GetSyncTrack(ReadFile(path).ToArray(), config);
        /// <summary>
        /// Gets the sync track from the contents of a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="SyncTrack"/>
        ///     <para><see langword="null"/> if the lines contain no sync track</para>
        /// </returns>
        /// <param name="lines">Lines in the file</param>
        /// <exception cref="FormatException"/>
        /// <inheritdoc cref="TrackObjectEntry(string)" path="/exception"/>
        /// <inheritdoc cref="GetPart(IEnumerable{string}, string)" path="/exception"/>
        private static SyncTrack? GetSyncTrack(string[] lines, ReadingConfiguration? config)
        {
            config ??= DefaultReadConfig;
            SyncTrack syncTrack = new();
            HashSet<uint> ignoredTempos = new(), ignoredAnchors = new(), ignoredSignatures = new();

            Func<uint, HashSet<uint>, bool> includeObject = config.DuplicateTrackObjectPolicy switch
            {
                DuplicateTrackObjectPolicy.IncludeAll => (_, _) => true,
                DuplicateTrackObjectPolicy.IncludeFirst => (position, ignored) =>
                {
                    if (ignored.Contains(position))
                        return false;

                    ignored.Add(position);
                    return true;
                }
                ,
                DuplicateTrackObjectPolicy.ThrowException => (position, ignored) =>
                {
                    if (ignored.Contains(position))
                        throw new Exception("Duplicate chord"); // TODO Make better exception;

                    ignored.Add(position);
                    return true;
                }
            };

            foreach (string line in GetPart(lines, "SyncTrack"))
            {
                TrackObjectEntry entry;
                try { entry = new(line); }
                catch (Exception e) { throw GetLineException(line, e); }

                Tempo? marker;

                switch (entry.Type)
                {
                    // Time signature
                    case "TS":
                        if (!includeObject(entry.Position, ignoredSignatures))
                            break;

                        string[] split = GetDataSplit(entry.Data);

                        byte denominator;

                        if (!byte.TryParse(split[0], out byte numerator))
                            throw new FormatException($"Cannot parse numerator \"{split[0]}\" to byte.");

                        // Denominator is only written if not equal to 4
                        if (split.Length < 2)
                            denominator = 4;
                        else
                        {
                            if (byte.TryParse(split[1], out denominator))
                                //Denominator is written as its second power
                                denominator = (byte)Math.Pow(2, denominator);
                            else
                                throw new FormatException($"Cannot parse denominator \"{split[1]}\" to byte.");
                        }

                        syncTrack.TimeSignatures.Add(new(entry.Position, numerator, denominator));
                        break;
                    // Tempo
                    case "B":
                        if (!includeObject(entry.Position, ignoredTempos))
                            break;

                        // Floats are written by rounding to the 3rd decimal and removing the decimal point
                        if (float.TryParse(entry.Data, out float value))
                            value /= 1000;
                        else
                            throw new FormatException($"Cannot parse value \"{entry.Data}\" to float.");

                        // Find the marker matching the position in case it was already added through a mention of anchor
                        marker = syncTrack.Tempo.FirstOrDefault(m => m.Position == entry.Position);

                        if (marker is null)
                            syncTrack.Tempo.Add(new(entry.Position, value));
                        else
                            marker.Value = value;
                        break;
                    // Anchor
                    case "A":
                        if (!includeObject(entry.Position, ignoredAnchors))
                            break;

                        // Floats are written by rounding to the 3rd decimal and removing the decimal point
                        if (float.TryParse(entry.Data, out float anchor))
                            anchor /= 1000;
                        else
                            throw new FormatException($"Cannot parse value \"{entry.Data}\" to float.");

                        // Find the marker matching the position in case it was already added through a mention of value
                        marker = syncTrack.Tempo.FirstOrDefault(m => m.Position == entry.Position);

                        if (marker is null)
                            syncTrack.Tempo.Add(new(entry.Position, 0) { Anchor = anchor });
                        else
                            marker.Anchor = anchor;

                        break;
                }
            }

            // Return null if no data
            return syncTrack.TimeSignatures.Count == 0 && syncTrack.Tempo.Count == 0 ? null : syncTrack;
        }

        /// <summary>
        /// Gets the lines from a text file.
        /// </summary>
        /// <returns>Enumerable of all the lines in the file</returns>
        /// <param name="path">Path of the file to read</param>
        /// <inheritdoc cref="StreamReader(string)" path="/exception"/>
        /// <inheritdoc cref="StreamReader.ReadLine" path="/exception"/>
        private static IEnumerable<string> ReadFile(string path)
        {
            using StreamReader reader = new(path);

            // Read to the end
            using (reader)
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();

                    if (line is not null or "")
                        yield return line;
                }
        }
        /// <summary>
        /// Gets a part from the contents of a chart file
        /// </summary>
        /// <returns>Enumerable of all the lines in the part</returns>
        /// <param name="lines">Lines in the file</param>
        /// <param name="partName">Name of the part to extract</param>
        /// <exception cref="InvalidDataException"/>
        /// <exception cref="InvalidOperationException"/>
        private static IEnumerable<string> GetPart(IEnumerable<string> lines, string partName)
        {
            using IEnumerator<string> enumerator = lines.GetEnumerator();
            enumerator.MoveNext();

            // Find part
            while (enumerator.Current != $"[{partName}]")
                if (!enumerator.MoveNext())
                    yield break;

            // Move past the part name and opening bracket
            for (int i = 0; i < 2; i++)
                if (!enumerator.MoveNext())
                    yield break;

            // Read until closing bracket
            while (enumerator.Current != "}")
            {
                yield return enumerator.Current;

                if (!enumerator.MoveNext())
                    throw new InvalidDataException($"Part \"{partName}\" did not end within the provided lines.");
            }
        }
    }
}
