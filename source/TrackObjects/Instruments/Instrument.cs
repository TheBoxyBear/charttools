using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Ini;
using ChartTools.SystemExtensions.Linq;

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

        /// <summary>
        /// Easy track
        /// </summary>
        public abstract Track Easy { get; }
        /// <summary>
        /// Medium track
        /// </summary>
        public abstract Track Medium { get; }
        /// <summary>
        /// Hard track
        /// </summary>
        public abstract Track Hard { get; }
        /// <summary>
        /// Expert track
        /// </summary>
        public abstract Track Expert { get; }
        /// <summary>
        /// Gets the track matching a difficulty.
        /// </summary>
        public virtual Track GetTrack(DiffEnum difficulty) => difficulty switch
        {
            DiffEnum.Easy => Easy,
            DiffEnum.Medium => Medium,
            DiffEnum.Hard => Hard,
            DiffEnum.Expert => Expert,
            _ => throw CommonExceptions.GetUndefinedException(difficulty)
        };
        public Track[] GetTracks() => new Track[] { Easy, Medium, Hard, Expert };

        #region File reading
        /// <summary>
        /// Reads an instrument from a file.
        /// </summary>
        /// <inheritdoc cref="ChartParser.ReadInstrument(string, Instruments, ReadingConfiguration)"/>
        public static Instrument FromFile(string path, Instruments instrument, ReadingConfiguration? config = default) => Enum.IsDefined(instrument)
            ? ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartParser.ReadInstrument(p, instrument, config)))
            : throw CommonExceptions.GetUndefinedException(instrument);

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
            : throw CommonExceptions.GetUndefinedException(instrument);

        /// <summary>
        /// Reads a standard instrument from a file.
        /// </summary>
        /// <inheritdoc cref="ChartParser.ReadInstrument(string, StandardInstrument, ReadingConfiguration)"/>
        public static Instrument<StandardChord> FromFile(string path, StandardInstrument instrument, ReadingConfiguration? config = default) => Enum.IsDefined(instrument)
            ? ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartParser.ReadInstrument(p, instrument, config)))
            : throw CommonExceptions.GetUndefinedException(instrument);
        #endregion

        /// <inheritdoc cref="IniParser.ReadDifficulty(string, Instruments)"/>
        public static sbyte? ReadDifficulty(string path, Instruments instrument) => ExtensionHandler.Read(path, (".ini", p => IniParser.ReadDifficulty(p, instrument)));
        /// <inheritdoc cref="IniParser.WriteDifficulty(string, Instruments, sbyte)"/>
        public static void WriteDifficulty(string path, Instruments instrument, sbyte value) => ExtensionHandler.Write(path, value, (".ini", (p, v) => IniParser.WriteDifficulty(p, instrument, v)));
    }
}
