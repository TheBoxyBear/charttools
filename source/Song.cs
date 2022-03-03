using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Ini;
using ChartTools.IO.MIDI;
using ChartTools.Lyrics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ChartTools.SystemExtensions.Linq;
using System.Threading.Tasks;
using System.Threading;
using ChartTools.IO.Configuration;
using ChartTools.Formatting;

namespace ChartTools
{
    /// <summary>
    /// Song playable in Clone Hero
    /// </summary>
    public class Song
    {
        /// <summary>
        /// Set of information about the song not unrelated to instruments, syncing or events
        /// </summary>
        public Metadata Metadata
        {
            get => _metadata;
            set => _metadata = value ?? throw new ArgumentNullException(nameof(value));
        }
        private Metadata _metadata = new();

        /// <inheritdoc cref="FormattingRules"/>
        public FormattingRules Formatting
        {
            get => _formatting;
            set => _formatting = value ?? throw new ArgumentNullException(nameof(value));
        }
        private FormattingRules _formatting = new();

        public Dictionary<string, string> UnidentifiedData { get; set; } = new();

        /// <inheritdoc cref="ChartTools.SyncTrack"/>
        public SyncTrack SyncTrack
        {
            get => _syncTrack;
            set => _syncTrack = value ?? throw new ArgumentNullException(nameof(value));
        }
        private SyncTrack _syncTrack = new();

        /// <summary>
        /// List of events common to all instruments
        /// </summary>
        public List<GlobalEvent> GlobalEvents
        {
            get => _globalEvents;
            set => _globalEvents = value ?? throw new ArgumentNullException(nameof(value));
        }
        private List<GlobalEvent> _globalEvents = new();

        #region Instruments
        /// <summary>
        /// Set of drums tracks
        /// </summary>
        public Instrument<DrumsChord>? Drums
        {
            get => _drums;
            set => _drums = value is null ? value : value with { InstrumentIdentity = InstrumentIdentity.Drums };
        }
        private Instrument<DrumsChord>? _drums;
        /// <summary>
        /// Set of Guitar Hero Live guitar tracks
        /// </summary>
        public Instrument<GHLChord>? GHLGuitar
        {
            get => _ghlGuitar;
            set => _ghlGuitar = value is null ? value : value with { InstrumentIdentity = InstrumentIdentity.GHLGuitar };
        }
        private Instrument<GHLChord>? _ghlGuitar;
        /// <summary>
        /// Set of Guitar Hero Live bass tracks
        /// </summary>
        public Instrument<GHLChord>? GHLBass
        {
            get => _ghlBass;
            set => _ghlBass = value with { InstrumentIdentity = InstrumentIdentity.GHLBass };
        }
        private Instrument<GHLChord>? _ghlBass;
        /// <summary>
        /// Set of lead guitar tracks
        /// </summary>
        public Instrument<StandardChord>? LeadGuitar
        {
            get => _leadGuitar;
            set => _leadGuitar = value is null ? value : value with { InstrumentIdentity = InstrumentIdentity.LeadGuitar };
        }
        private Instrument<StandardChord>? _leadGuitar;
        /// <summary>
        /// Set of rhythm guitar tracks
        /// </summary>
        public Instrument<StandardChord>? RhythmGuitar
        {
            get => _rhythmGuitar;
            set => _rhythmGuitar = value is null ? value : value with { InstrumentIdentity = InstrumentIdentity.RhythmGuitar };
        }
        private Instrument<StandardChord>? _rhythmGuitar;
        /// <summary>
        /// Set of coop guitar tracks
        /// </summary>
        public Instrument<StandardChord>? CoopGuitar
        {
            get => _coopGuitar;
            set => _coopGuitar = value is null ? value : value with { InstrumentIdentity = InstrumentIdentity.CoopGuitar };
        }
        private Instrument<StandardChord>? _coopGuitar;
        /// <summary>
        /// Set of bass tracks
        /// </summary>
        public Instrument<StandardChord>? Bass
        {
            get => _bass;
            set => _bass = value is null ? value : value with { InstrumentIdentity = InstrumentIdentity.Bass };
        }
        private Instrument<StandardChord>? _bass;
        /// <summary>
        /// Set of keyboard tracks
        /// </summary>
        public Instrument<StandardChord>? Keys
        {
            get => _keys;
            set => _keys = value is null ? value : value with { InstrumentIdentity = InstrumentIdentity.Keys };
        }
        private Instrument<StandardChord>? _keys;
        public Instrument<Phrase>? Vocals
        {
            get => _vocals;
            set => _vocals = value is null ? value : value with { InstrumentIdentity = InstrumentIdentity.Vocals };
        }
        private Instrument<Phrase>? _vocals;
        #endregion

        /// <summary>
        /// Gets property value for an <see cref="Instrument"/> from a <see cref="InstrumentIdentity"/> <see langword="enum"/> value.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument"/> from the <see cref="Song"/></returns>
        /// <param name="instrument">Instrument to get</param>
        public Instrument? GetInstrument(InstrumentIdentity instrument) => instrument switch
        {
            InstrumentIdentity.Drums => Drums,
            InstrumentIdentity.GHLGuitar => GHLGuitar,
            InstrumentIdentity.GHLBass => GHLBass,
            InstrumentIdentity.LeadGuitar => LeadGuitar,
            InstrumentIdentity.RhythmGuitar => RhythmGuitar,
            InstrumentIdentity.CoopGuitar => CoopGuitar,
            InstrumentIdentity.Bass => Bass,
            InstrumentIdentity.Keys => Keys,
            InstrumentIdentity.Vocals => Vocals,
            _ => throw new Exception("Instrument does not exist.")
        };
        /// <summary>
        /// Gets property value for an <see cref="Instrument{TChord}"/> from a <see cref="GHLInstrumentIdentity"/> <see langword="enum"/> value.
        /// </summary>
        /// /// <param name="instrument">Instrument to get</param>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="GHLChord"/> from the <see cref="Song"/>.</returns>
        public Instrument<GHLChord>? GetInstrument(GHLInstrumentIdentity instrument) => GetInstrument((InstrumentIdentity)instrument) as Instrument<GHLChord>;
        /// <summary>
        /// Gets property value for an <see cref="Instrument{TChord}"/> from a <see cref="StandardInstrumentIdentity"/> <see langword="enum"/> value.
        /// </summary>
        /// <param name="instrument">Instrument to get</param>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="StandardChord"/> from the <see cref="Song"/>.</returns>
        public Instrument<StandardChord>? GetInstrument(StandardInstrumentIdentity instrument) => GetInstrument((InstrumentIdentity)instrument) as Instrument<StandardChord>;
        public IEnumerable<Instrument?> GetInstruments() => new Instrument?[] { Drums, GHLGuitar, GHLBass, LeadGuitar, RhythmGuitar, CoopGuitar, Bass, Keys }.NonNull();

        public void SetInstrument(Instrument<StandardChord> instrument, StandardInstrumentIdentity identity)
        {
            switch (identity)
            {
                case StandardInstrumentIdentity.LeadGuitar:
                    LeadGuitar = instrument;
                    break;
                case StandardInstrumentIdentity.RhythmGuitar:
                    RhythmGuitar = instrument;
                    break;
                case StandardInstrumentIdentity.CoopGuitar:
                    CoopGuitar = instrument;
                    break;
                case StandardInstrumentIdentity.Bass:
                    Bass = instrument;
                    break;
                case StandardInstrumentIdentity.Keys:
                    Keys = instrument;
                    break;
            }
        }
        public void SetInstrument(Instrument<GHLChord> instrument, GHLInstrumentIdentity identity)
        {
            switch (identity)
            {
                case GHLInstrumentIdentity.Guitar:
                    GHLGuitar = instrument;
                    break;
                case GHLInstrumentIdentity.Bass:
                    GHLBass = instrument;
                    break;
            }
        }

        /// <summary>
        /// Reads all elements of a <see cref="Song"/> from a file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        public static Song FromFile(string path, ReadingConfiguration? config = default) => ExtensionHandler.Read(path, config, (".chart", ChartFile.ReadSong), (".ini", (p, config) => new Song { Metadata = IniParser_old.ReadMetadata(p) }));

        /// <summary>
        /// Reads all elements of a <see cref="Song"/> from a file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="FromFile(string, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="cancellationToken">Token to request cancellation</param>
        /// <param name="config"><inheritdoc cref="FromFile(string, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        public static async Task<Song> FromFileAsync(string path, CancellationToken cancellationToken, ReadingConfiguration? config = default) => await ExtensionHandler.ReadAsync<Song>(path, cancellationToken, config, (".chart", ChartFile.ReadSongAsync));

        /// <summary>
        /// Writes the <see cref="Song"/> to a file.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="System.Security.SecurityException"/>
        public void ToFile(string path, WritingConfiguration? config = default) => ExtensionHandler.Write(path, this, config, (".chart", ChartFile.WriteSong));
        public async Task ToFileAsync(string path, CancellationToken cancellationToken, WritingConfiguration? config = default) => await ExtensionHandler.WriteAsync(path, this, cancellationToken, config, (".chart", ChartFile.WriteSongAsync));

        /// <summary>
        /// Reads the estimated instrument difficulties from a ini file.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        public void ReadDifficulties(string path) => ExtensionHandler.Read(path, (".ini", path => IniParser_old.ReadDifficulties(path, this)));
        /// <summary>
        /// Writes the estimated instrument difficulties to a ini file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="IOException"/>
        public void WriteDifficulties(string path) => ExtensionHandler.Write(path, (".ini", path => IniParser_old.WriteDifficulties(path, this)));

        /// <summary>
        /// Retrieves the lyrics from the global events.
        /// </summary>
        public IEnumerable<Phrase> GetLyrics() => GlobalEvents is null ? Enumerable.Empty<Phrase>() : GlobalEvents.GetLyrics();
        /// <summary>
        /// Replaces phrase and lyric events from <see cref="GlobalEvents"/> with the ones making up a set of <see cref="Phrase"/>.
        /// </summary>
        /// <param name="phrases">Phrases to use as a replacement</param>
        public void SetLyrics(IEnumerable<Phrase> phrases) => GlobalEvents = (GlobalEvents ?? new()).SetLyrics(phrases).ToList();
    }
}
