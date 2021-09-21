using ChartTools.IO;
using ChartTools.IO.Chart;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools
{
    /// <summary>
    /// Base class for tracks
    /// </summary>
    public abstract class Track
    {
        /// <summary>
        /// Events specific to the <see cref="Track"/>
        /// </summary>
        public List<LocalEvent>? LocalEvents { get; set; } = new();
        /// <summary>
        /// Sets of star power phrases
        /// </summary>
        public UniqueTrackObjectCollection<StarPowerPhrase> StarPower { get; set; } = new();

        public abstract IEnumerable<Chord> GetChords();

        internal IEnumerable<StarPowerPhrase> SoloToStarPower(bool removeEvents)
        {
            if (LocalEvents is null)
                yield break;

            foreach (LocalEvent e in LocalEvents.OrderBy(e => e.Position))
            {
                StarPowerPhrase? phrase = null;

                switch (e.EventType)
                {
                    case LocalEventType.Solo:
                        phrase = new(e.Position);
                        break;
                    case LocalEventType.SoloEnd:
                        if (phrase is not null)
                        {
                            phrase.Length = e.Position - phrase.Position;
                            yield return phrase;
                            phrase = null;
                        }
                        break;
                }
            }

            if (removeEvents)
                LocalEvents.RemoveAll(e => e.IsStarPowerEvent);
        }

        #region File reading
        /// <summary>
        /// Reads a track from a file
        /// </summary>
        /// <inheritdoc cref="ChartParser.ReadTrack(string, Instruments, Difficulty, ReadingConfiguration)"/>
        public static Track FromFile(string path, Instruments instrument, Difficulty difficulty, ReadingConfiguration? config = default) => ExtensionHandler.Read(path, config, (".chart", (p, c) => ChartParser.ReadTrack(p, instrument, difficulty, c)));

        /// <inheritdoc cref="ChartParser.ReadDrumsTrack(string, Difficulty, ReadingConfiguration)"/>
        public static Track<DrumsChord> FromFile(string path, Difficulty difficulty, ReadingConfiguration? config = default) => ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartParser.ReadDrumsTrack(p, difficulty, config)));

        /// <inheritdoc cref="ChartParser.ReadTrack(string, GHLInstrument, Difficulty)"/>
        public static Track<GHLChord> FromFile(string path, GHLInstrument instrument, Difficulty difficulty, ReadingConfiguration? config = default) => ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartParser.ReadTrack(p, instrument, difficulty, config)));

        /// <inheritdoc cref="ChartParser.ReadTrack(string, StandardInstrument, Difficulty)"/>
        public static Track<StandardChord> FromFile(string path, StandardInstrument instrument, Difficulty difficulty, ReadingConfiguration? config = default) => ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartParser.ReadTrack(p, instrument, difficulty, config)));
        #endregion
    }
}
