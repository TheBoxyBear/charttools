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
        public Metadata? Metadata { get; set; } = new();
        /// <inheritdoc cref="ChartTools.SyncTrack"/>
        public SyncTrack? SyncTrack { get; set; } = new();
        /// <summary>
        /// List of events common to all instruments
        /// </summary>
        public List<GlobalEvent>? GlobalEvents { get; set; } = new();

        #region Instruments
        /// <summary>
        /// Set of drums tracks
        /// </summary>
        public Instrument<DrumsChord>? Drums
        {
            get => _drums;
            set => _drums = value is null ? value : value with
            {
                InstrumentIdentity = Instruments.Drums,
                InstrumentType = InstrumentType.Drums
            };
        }
        private Instrument<DrumsChord>? _drums;
        /// <summary>
        /// Set of Guitar Hero Live guitar tracks
        /// </summary>
        public Instrument<GHLChord>? GHLGuitar
        {
            get => _ghlGuitar;
            set => _ghlGuitar = value is null ? value : value with
            {
                InstrumentIdentity = Instruments.GHLGuitar,
                InstrumentType = InstrumentType.GHL
            };
        }
        private Instrument<GHLChord>? _ghlGuitar;
        /// <summary>
        /// Set of Guitar Hero Live bass tracks
        /// </summary>
        public Instrument<GHLChord>? GHLBass
        {
            get => _ghlBass;
            set => _ghlBass = value with
            {
                InstrumentIdentity = Instruments.GHLBass,
                InstrumentType = InstrumentType.GHL
            };
        }
        private Instrument<GHLChord>? _ghlBass;
        /// <summary>
        /// Set of lead guitar tracks
        /// </summary>
        public Instrument<StandardChord>? LeadGuitar
        {
            get => _leadGuitar;
            set => _leadGuitar = value is null ? value : value with
            {
                InstrumentIdentity = Instruments.LeadGuitar,
                InstrumentType = InstrumentType.Standard
            };
        }
        private Instrument<StandardChord>? _leadGuitar;
        /// <summary>
        /// Set of rhythm guitar tracks
        /// </summary>
        public Instrument<StandardChord>? RhythmGuitar
        {
            get => _rhythmGuitar;
            set => _rhythmGuitar = value is null ? value : value with
            {
                InstrumentIdentity = Instruments.RhythmGuitar,
                InstrumentType = InstrumentType.Standard
            };
        }
        private Instrument<StandardChord>? _rhythmGuitar;
        /// <summary>
        /// Set of coop guitar tracks
        /// </summary>
        public Instrument<StandardChord>? CoopGuitar
        {
            get => _coopGuitar;
            set => _coopGuitar = value is null ? value : value with
            {
                InstrumentIdentity = Instruments.CoopGuitar,
                InstrumentType = InstrumentType.Standard
            };
        }
        private Instrument<StandardChord>? _coopGuitar;
        /// <summary>
        /// Set of bass tracks
        /// </summary>
        public Instrument<StandardChord>? Bass
        {
            get => _bass;
            set => _bass = value is null ? value : value with
            {
                InstrumentIdentity = Instruments.Bass,
                InstrumentType = InstrumentType.Standard
            };
        }
        private Instrument<StandardChord>? _bass;
        /// <summary>
        /// Set of keyboard tracks
        /// </summary>
        public Instrument<StandardChord>? Keys
        {
            get => _keys;
            set => _keys = value is null ? value : value with
            {
                InstrumentIdentity = Instruments.Keys,
                InstrumentType = InstrumentType.Standard
            };
        }
        private Instrument<StandardChord>? _keys;
        public Instrument<Phrase>? Vocals
        {
            get => _vocals;
            set => _vocals = value is null ? value : value with
            {
                InstrumentIdentity = Instruments.Vocals,
                InstrumentType = InstrumentType.Vocals
            };
        }
        private Instrument<Phrase>? _vocals;
        #endregion

        /// <summary>
        /// Gets property value for an <see cref="Instrument"/> from a <see cref="Instruments"/> <see langword="enum"/> value.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument"/> from the <see cref="Song"/></returns>
        /// <param name="instrument">Instrument to get</param>
        public Instrument? GetInstrument(Instruments instrument) => instrument switch
        {
            Instruments.Drums => Drums,
            Instruments.GHLGuitar => GHLGuitar,
            Instruments.GHLBass => GHLBass,
            Instruments.LeadGuitar => LeadGuitar,
            Instruments.RhythmGuitar => RhythmGuitar,
            Instruments.CoopGuitar => CoopGuitar,
            Instruments.Bass => Bass,
            Instruments.Keys => Keys,
            Instruments.Vocals => Vocals,
            _ => throw new Exception("Instrument does not exist.")
        };
        /// <summary>
        /// Gets property value for an <see cref="Instrument{TChord}"/> from a <see cref="GHLInstrument"/> <see langword="enum"/> value.
        /// </summary>
        /// /// <param name="instrument">Instrument to get</param>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="GHLChord"/> from the <see cref="Song"/>.</returns>
        public Instrument<GHLChord>? GetInstrument(GHLInstrument instrument) => GetInstrument((Instruments)instrument) as Instrument<GHLChord>;
        /// <summary>
        /// Gets property value for an <see cref="Instrument{TChord}"/> from a <see cref="StandardInstrument"/> <see langword="enum"/> value.
        /// </summary>
        /// <param name="instrument">Instrument to get</param>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="StandardChord"/> from the <see cref="Song"/>.</returns>
        public Instrument<StandardChord>? GetInstrument(StandardInstrument instrument) => GetInstrument((Instruments)instrument) as Instrument<StandardChord>;
        public IEnumerable<Instrument?> GetInstruments() => new Instrument?[] { Drums, GHLGuitar, GHLBass, LeadGuitar, RhythmGuitar, CoopGuitar, Bass, Keys }.NonNull();

        public void SetInstrument(Instrument<StandardChord> instrument, StandardInstrument identity)
        {
            switch (identity)
            {
                case StandardInstrument.LeadGuitar:
                    LeadGuitar = instrument;
                    break;
                case StandardInstrument.RhythmGuitar:
                    RhythmGuitar = instrument;
                    break;
                case StandardInstrument.CoopGuitar:
                    CoopGuitar = instrument;
                    break;
                case StandardInstrument.Bass:
                    Bass = instrument;
                    break;
                case StandardInstrument.Keys:
                    Keys = instrument;
                    break;
            }
        }
        public void SetInstrument(Instrument<GHLChord> instrument, GHLInstrument identity)
        {
            switch (identity)
            {
                case GHLInstrument.Guitar:
                    GHLGuitar = instrument;
                    break;
                case GHLInstrument.Bass:
                    GHLBass = instrument;
                    break;
            }
        }

        /// <summary>
        /// Reads a <see cref="Song"/> from a file.
        /// </summary>
        /// <remarks>Supported extensions: chart, ini (mid currently in development)</remarks>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        /// <exception cref="CommonExceptions.ParameterNullException"/>
        public static Song FromFile(string path, ReadingConfiguration? config = default) => ExtensionHandler.Read(path, config, (".mid", MIDIParser.ReadSong), (".chart", ChartParser.ReadSong), (".ini", (p, config) => new Song { Metadata = IniParser.ReadMetadata(p) }));

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
        public void ToFile(string path, WritingConfiguration? config = default) => ExtensionHandler.Write(path, this, config, (".chart", ChartParser.WriteSong));

        /// <summary>
        /// Reads the estimated instrument difficulties from a ini file.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        public void ReadDifficulties(string path) => ExtensionHandler.Read(path, (".ini", p => IniParser.ReadDifficulties(p, this)));
        /// <summary>
        /// Writes the estimated instrument difficulties to a ini file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="IOException"/>
        public void WriteDifficulties(string path) => ExtensionHandler.Write(path, this, (".ini", IniParser.WriteDifficulties));

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
