using ChartTools.Internal;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Configuration;

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
        /// <inheritdoc cref="IEmpty.IsEmpty"/>
        public bool IsEmpty => Chords.Count == 0 && LocalEvents.Count == 0 && StarPower.Count == 0;

        /// <summary>
        /// Difficulty of the track
        /// </summary>
        public Difficulty Difficulty { get; init; }
        /// <summary>
        /// Instrument containing the track
        /// </summary>
        public Instrument? ParentInstrument { get; protected set; }
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
                        phrase = new(e.Position, SpecialPhraseType.StarPowerGain);
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
        /// Reads a track from a file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="instrument">Instrument of the track</param>
        /// <param name="difficulty">Difficulty of the track</param>
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        public static Track FromFile(string path, InstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default) => ExtensionHandler.Read<Track>(path, config, (".chart", (p, c) => ChartReader.ReadTrack(path, instrument, difficulty, config)));
        /// <summary>
        /// Reads a track from a file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='difficulty']"/></param>
        /// <param name="cancellationToken">Token to request cancellation</param>
        /// <param name="config"><inheritdoc cref="FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        /// <returns></returns>
        public static async Task<Track> FromFileAsync(string path, InstrumentIdentity instrument, Difficulty difficulty, CancellationToken cancellationToken, ReadingConfiguration? config = default) => await ExtensionHandler.ReadAsync<Track>(path, cancellationToken, config, (".chart", (path, token, config) => ChartReader.ReadTrackAsync(path, instrument, difficulty, token, config)));

        /// <summary>
        /// Reads a drums track from a file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="difficulty">Difficulty of the track</param>
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        public static Track<DrumsChord> FromFile(string path, Difficulty difficulty, ReadingConfiguration? config = default) => ExtensionHandler.Read<Track<DrumsChord>>(path, config, (".chart", (path, config) => ChartReader.ReadDrumsTrack(path, difficulty, config)));
        /// <summary>
        /// Reads a drums track from a file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="FromFile(string, Difficulty, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="difficulty"><inheritdoc cref="FromFile(string, Difficulty, ReadingConfiguration?)" path="/param[@name='difficulty']"/></param>
        /// <param name="cancellationToken">Token to request cancellation</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task<Track<DrumsChord>> FromFileAsync(string path, Difficulty difficulty, CancellationToken cancellationToken, ReadingConfiguration? config = default) => await ExtensionHandler.ReadAsync<Track<DrumsChord>>(path, cancellationToken, config, (".chart", (path, token, config) => ChartReader.ReadDrumsTrackAsync(path, difficulty, cancellationToken, config)));

        /// <summary>
        /// Reads a GHL track from a file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="instrument">GHL instrument of the track</param>
        /// <param name="difficulty">Difficulty of the track</param>
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        public static Track<GHLChord> FromFile(string path, GHLInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default) => ExtensionHandler.Read<Track<GHLChord>>(path, config, (".chart", (path, config) => ChartReader.ReadTrack(path, instrument, difficulty, config)));
        /// <summary>
        /// Reads a GHL track from a file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="FromFile(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="FromFile(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="FromFile(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='difficulty']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="FromFile(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="FromFile(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        /// <returns></returns>
        public static async Task<Track<GHLChord>> FromFileAsync(string path, GHLInstrumentIdentity instrument, Difficulty difficulty, CancellationToken cancellationToken, ReadingConfiguration? config = default) => await ExtensionHandler.ReadAsync<Track<GHLChord>>(path, cancellationToken, config, (".chart", (path, token, config) => ChartReader.ReadTrackAsync(path, instrument, difficulty, cancellationToken, config)));

        /// <summary>
        /// Reads a standard track from a file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="instrument">Standard instrument of the track</param>
        /// <param name="difficulty">Difficulty of the track</param>
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        public static Track<StandardChord> FromFile(string path, StandardInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default) => ExtensionHandler.Read<Track<StandardChord>>(path, config, (".chart", (path, config) => ChartReader.ReadTrack(path, instrument, difficulty, config)));
        /// <summary>
        /// Reads a track from a file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='difficulty']"/></param>
        /// <param name="cancellationToken">Token to request cancellation</param>
        /// <param name="config"><inheritdoc cref="FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        public static async Task<Track<StandardChord>> FromFileAsync(string path, StandardInstrumentIdentity instrument, Difficulty difficulty, CancellationToken cancellationToken, ReadingConfiguration? config = default) => await ExtensionHandler.ReadAsync<Track<StandardChord>>(path, cancellationToken, config, (".chart", (path, token, config) => ChartReader.ReadTrackAsync(path, instrument, difficulty, cancellationToken, config)));
        #endregion

        public void ToFile(string path, WritingConfiguration? config = default) => ExtensionHandler.Write<Track>(path, this, config, (".chart", ChartWriter.ReplaceTrack));
        public async Task ToFileAsync(string path, CancellationToken cancellationToken, WritingConfiguration? config = default) => await ExtensionHandler.WriteAsync<Track>(path, this, cancellationToken, config, (".chart", ChartWriter.ReplaceTrackAsync));
    }
}
