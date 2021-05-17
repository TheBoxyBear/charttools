using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Ini;
using System;

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

        /// <inheritdoc cref="ChartParser.ReadInstrument(string, Instruments)"/>
        public static Instrument FromFile(string path, Instruments instrument, ReadingConfiguration config)
        {
            if (!Enum.IsDefined(typeof(Instruments), instrument))
                throw new ArgumentException("Instrument is not defined.");

            try { return ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartParser.ReadInstrument(p, instrument, config))); }
            catch { throw; }
        }
        /// <inheritdoc cref="ChartParser.ReadDrums(string)"/>
        public static Instrument<DrumsChord> DrumsFromFile(string path, ReadingConfiguration config) => ExtensionHandler.Read(path, config, (".chart", ChartParser.ReadDrums));
        /// <inheritdoc cref="ChartParser.ReadInstrument(string, GHLInstrument)"/>
        public static Instrument<GHLChord> FromFile(string path, GHLInstrument instrument, ReadingConfiguration config)
        {
            if (!Enum.IsDefined(typeof(GHLInstrument), instrument))
                throw new ArgumentException("Instrument is not defined.");

            try { return ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartParser.ReadInstrument(p, instrument, config))); }
            catch { throw; }
        }
        /// <inheritdoc cref="ChartParser.ReadInstrument(string, StandardInstrument)"/>
        public static Instrument<StandardChord> FromFile(string path, StandardInstrument instrument, ReadingConfiguration config)
        {
            if (!Enum.IsDefined(typeof(StandardInstrument), instrument))
                throw new ArgumentException("Instrument is not defined.");

            try { return ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartParser.ReadInstrument(p, instrument, config))); }
            catch { throw; }
        }

        /// <inheritdoc cref="IniParser.ReadDifficulty(string, Instruments)"/>
        public static sbyte? ReadDifficulty(string path, Instruments instrument)
        {
            try { return ExtensionHandler.Read(path, (".ini", p => IniParser.ReadDifficulty(p, instrument))); }
            catch { throw; }
        }
        /// <inheritdoc cref="IniParser.WriteDifficulty(string, Instruments, sbyte)"/>
        public static void WriteDifficulty(string path, Instruments instrument, sbyte value)
        {
            try { ExtensionHandler.Write(path, value, (".ini", (p, v) => IniParser.WriteDifficulty(p, instrument, v))); }
            catch { throw; }
        }
    }
}
