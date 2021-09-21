using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Ini;

using System;
using System.Collections.Generic;

using DiffEnum = ChartTools.Difficulty;

namespace ChartTools
{
    /// <summary>
    /// Base class for instruments
    /// </summary>
    public abstract class Instrument
    {
        /// <summary>
        /// Estimated difficulty
        /// </summary>
        public sbyte? Difficulty { get; set; }

        private const string undefinedInstrumentMessage = "Instrument is not defined.";

        /// <summary>
        /// Gets the easy track.
        /// </summary>
        protected abstract Track? GetEasy();
        /// <summary>
        /// Gets the medium track.
        /// </summary>
        protected abstract Track? GetMedium();
        /// <summary>
        /// Gets the hard track.
        /// </summary>
        protected abstract Track? GetHard();
        /// <summary>
        /// Gets the expert track.
        /// </summary>
        protected abstract Track? GetExpert();
        /// <summary>
        /// Gets the track matching a difficulty.
        /// </summary>
        public Track? GetTrack(DiffEnum difficulty) => difficulty switch
        {
            DiffEnum.Easy => GetEasy(),
            DiffEnum.Medium => GetMedium(),
            DiffEnum.Hard => GetHard(),
            DiffEnum.Expert => GetExpert(),
            _ => throw CommonExceptions.GetUndefinedException(difficulty)
        };
        public IEnumerable<Track> GetTracks()
        {
            foreach (var getter in new Func<Track?>[] { GetEasy, GetMedium, GetHard, GetExpert })
            {
                Track? track = getter();

                if (track is not null)
                    yield return track;
            }
        }

        #region File reading
        /// <summary>
        /// Reads an instrument from a file.
        /// </summary>
        /// <inheritdoc cref="ChartParser.ReadInstrument(string, Instruments, ReadingConfiguration)"/>
        public static Instrument FromFile(string path, Instruments instrument, ReadingConfiguration? config = default) => Enum.IsDefined(instrument)
            ? ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartParser.ReadInstrument(p, instrument, config)))
            : throw new ArgumentException(undefinedInstrumentMessage);

        /// <summary>
        /// Reads drums from a file.
        /// </summary>
        /// <inheritdoc cref="ChartParser.ReadDrums(string, ReadingConfiguration)(string, Instruments, ReadingConfiguration)"/>
        public static Instrument<DrumsChord> FromFile(string path, ReadingConfiguration? config = default) => ExtensionHandler.Read(path, config, (".chart", ChartParser.ReadDrums));

        /// <summary>
        /// Reads a GHL instrument from a file.
        /// </summary>
        /// <inheritdoc cref="ChartParser.ReadInstrument(string, GHLInstrument, ReadingConfiguration)" />
        public static Instrument<GHLChord> FromFile(string path, GHLInstrument instrument, ReadingConfiguration? config = default) => Enum.IsDefined(instrument)
            ? ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartParser.ReadInstrument(p, instrument, config)))
            : throw new ArgumentException(undefinedInstrumentMessage);

        /// <summary>
        /// Reads a standard instrument from a file.
        /// </summary>
        /// <inheritdoc cref="ChartParser.ReadInstrument(string, StandardInstrument, ReadingConfiguration)"/>
        public static Instrument<StandardChord> FromFile(string path, StandardInstrument instrument, ReadingConfiguration? config = default) => Enum.IsDefined(instrument)
            ? ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartParser.ReadInstrument(p, instrument, config)))
            : throw new ArgumentException(undefinedInstrumentMessage);
        #endregion

        /// <inheritdoc cref="IniParser.ReadDifficulty(string, Instruments)"/>
        public static sbyte? ReadDifficulty(string path, Instruments instrument) => ExtensionHandler.Read(path, (".ini", p => IniParser.ReadDifficulty(p, instrument)));
        /// <inheritdoc cref="IniParser.WriteDifficulty(string, Instruments, sbyte)"/>
        public static void WriteDifficulty(string path, Instruments instrument, sbyte value) => ExtensionHandler.Write(path, value, (".ini", (p, v) => IniParser.WriteDifficulty(p, instrument, v)));
    }
}
