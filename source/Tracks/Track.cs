using ChartTools.Internal;
using ChartTools.IO;
using ChartTools.IO.Chart;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools
{
    /// <summary>
    /// Base class for tracks
    /// </summary>
    public abstract record Track : IEmpty
    {
        public bool IsEmpty => Chords.Count == 0 && LocalEvents.Count == 0 && StarPower.Count == 0;

        public Difficulty Difficulty { get; init; }
        public Instrument ParentInstrument { get; init; }

        /// <summary>
        /// Events specific to the <see cref="Track"/>
        /// </summary>
        public List<LocalEvent> LocalEvents { get; } = new();
        /// <summary>
        /// Sets of star power phrases
        /// </summary>
        public List<SpecicalPhrase> StarPower { get; } = new();

        /// <summary>
        /// Groups of notes of the same position
        /// </summary>
        public abstract IReadOnlyList<Chord> Chords { get; }

        internal IEnumerable<SpecicalPhrase> SoloToStarPower(bool removeEvents)
        {
            if (LocalEvents is null)
                yield break;

            foreach (LocalEvent e in LocalEvents.OrderBy(e => e.Position))
            {
                SpecicalPhrase? phrase = null;

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
        /// <inheritdoc cref="ChartWriter.ReadTrack(string, InstrumentIdentity, Difficulty, ReadingConfiguration)"/>
        public static Track FromFile(string path, InstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default) => ExtensionHandler.Read<Track>(path, config, (".chart", (p, c) => ChartReader.ReadTrack(path, instrument, difficulty, config)));
        public static async Task<Track> FromFileAsync(string path, InstrumentIdentity instrument, Difficulty difficulty, CancellationToken cancellationToken, ReadingConfiguration? config = default) => await ExtensionHandler.ReadAsync<Track>(path, cancellationToken, config, (".chart", (path, token, config) => ChartReader.ReadTrackAsync(path, instrument, difficulty, token, config)));

        /// <inheritdoc cref="ChartWriter.ReadDrumsTrack(string, Difficulty, ReadingConfiguration)"/>
        public static Track<DrumsChord> FromFile(string path, Difficulty difficulty, ReadingConfiguration? config = default) => ExtensionHandler.Read<Track<DrumsChord>>(path, config, (".chart", (path, config) => ChartReader.ReadDrumsTrack(path, difficulty, config)));
        public static async Task<Track<DrumsChord>> FromFile(string path, Difficulty difficulty, CancellationToken cancellationToken, ReadingConfiguration? config) => await ExtensionHandler.ReadAsync<Track<DrumsChord>>(path, cancellationToken, config, (".chart", (path, token, config) => ChartReader.ReadDrumsTrackAsync(path, difficulty, cancellationToken, config)));

        /// <inheritdoc cref="ChartWriter.ReadTrack(string, GHLInstrumentIdentity, Difficulty)"/>
        public static Track<GHLChord> FromFile(string path, GHLInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default) => ExtensionHandler.Read<Track<GHLChord>>(path, config, (".chart", (path, config) => ChartReader.ReadTrack(path, instrument, difficulty, config)));
        public static async Task<Track<GHLChord>> FromFileAsync(string path, GHLInstrumentIdentity instrument, Difficulty difficulty, CancellationToken cancellationToken, ReadingConfiguration? config) => await ExtensionHandler.ReadAsync<Track<GHLChord>>(path, cancellationToken, config, (".chart", (path, token, config) => ChartReader.ReadTrackAsync(path, instrument, difficulty, cancellationToken, config)));

        /// <inheritdoc cref="ChartWriter.ReadTrack(string, StandardInstrumentIdentity, Difficulty)"/>
        public static Track<StandardChord> FromFile(string path, StandardInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default) => ExtensionHandler.Read<Track<StandardChord>>(path, config, (".chart", (path, config) => ChartReader.ReadTrack(path, instrument, difficulty, config)));
        public static async Task<Track<StandardChord>> FromFileAsync(string path, StandardInstrumentIdentity instrument, Difficulty difficulty, CancellationToken cancellationToken, ReadingConfiguration? config) => await ExtensionHandler.ReadAsync<Track<StandardChord>>(path, cancellationToken, config, (".chart", (path, token, config) => ChartReader.ReadTrackAsync(path, instrument, difficulty, cancellationToken, config)));
        #endregion

        public void ToFile(string path, WritingConfiguration? config = default) => ExtensionHandler.Write<Track>(path, this, config, (".chart", ChartWriter.ReplaceTrack));
        public async Task ToFileAsync(string path, CancellationToken cancellationToken, WritingConfiguration? config = default) => await ExtensionHandler.WriteAsync<Track>(path, this, cancellationToken, config, (".chart", ChartWriter.ReplaceTrackAsync));
    }
}
