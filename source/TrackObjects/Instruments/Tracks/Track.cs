using ChartTools.Collections.Unique;
using ChartTools.IO;
using ChartTools.IO.Chart;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools
{
    /// <summary>
    /// Base class for tracks
    /// </summary>
    public class Track
    {
        /// <summary>
        /// Events specific to the <see cref="Track"/>
        /// </summary>
        public List<LocalEvent> LocalEvents { get; set; } = new List<LocalEvent>();
        /// <summary>
        /// Sets of chords that give star power
        /// </summary>
        public UniqueList<StarPowerPhrase> StarPower { get; set; } = new UniqueList<StarPowerPhrase>((s, other) => s.Equals(other));

        internal IEnumerable<StarPowerPhrase> SoloToStarPower(bool removeEvents)
        {
            foreach (LocalEvent e in LocalEvents.OrderBy(e => e.Position))
            {
                StarPowerPhrase phrase = null;

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

        public static Track FromFile(string path, Instruments instrument, Difficulty difficulty, ReadingConfiguration config) => ExtensionHandler.Read(path, config, (".chart", (p, c) => ChartParser.ReadTrack(p, instrument, difficulty, c)));
        /// <inheritdoc cref="ChartParser.ReadDrumsTrack(string, Difficulty)"/>
        public static Track<DrumsChord> FromFile(string path, Difficulty difficulty, ReadingConfiguration config) => ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartParser.ReadDrumsTrack(p, difficulty, config)));
        /// <inheritdoc cref="ChartParser.ReadTrack(string, GHLInstrument, Difficulty)"/>
        public static Track<GHLChord> FromFile(string path, GHLInstrument instrument, Difficulty difficulty, ReadingConfiguration config) => ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartParser.ReadTrack(p, instrument, difficulty, config)));
        /// <inheritdoc cref="ChartParser.ReadTrack(string, StandardInstrument, Difficulty)"/>
        public static Track<StandardChord> FromFile(string path, StandardInstrument instrument, Difficulty difficulty, ReadingConfiguration config) => ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartParser.ReadTrack(p, instrument, difficulty, config)));
    }
}
