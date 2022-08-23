using ChartTools.Extensions.Linq;
using ChartTools.Lyrics;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools
{
    /// <summary>
    /// Set of all instruments
    /// </summary>
    public class InstrumentSet : IEnumerable<Instrument>
    {
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
            set => _ghlBass = value is null ? value : value with { InstrumentIdentity = InstrumentIdentity.GHLBass };
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

        /// <summary>
        /// Gets property value for an <see cref="Instrument"/> from a <see cref="InstrumentIdentity"/> <see langword="enum"/> value.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument"/> from the <see cref="Song"/></returns>
        /// <param name="instrument">Instrument to get</param>
        public Instrument? Get(InstrumentIdentity instrument) => instrument switch
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
        public Instrument<GHLChord>? Get(GHLInstrumentIdentity instrument) => Get((InstrumentIdentity)instrument) as Instrument<GHLChord>;
        /// <summary>
        /// Gets property value for an <see cref="Instrument{TChord}"/> from a <see cref="StandardInstrumentIdentity"/> <see langword="enum"/> value.
        /// </summary>
        /// <param name="instrument">Instrument to get</param>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="StandardChord"/> from the <see cref="Song"/>.</returns>
        public Instrument<StandardChord>? Get(StandardInstrumentIdentity instrument) => Get((InstrumentIdentity)instrument) as Instrument<StandardChord>;

        public IEnumerable<Instrument> Existing() => this.NonNull().Where(instrument => !instrument.IsEmpty);

        public void Set(Instrument<StandardChord> instrument)
        {
            switch (instrument.InstrumentIdentity)
            {
                case InstrumentIdentity.LeadGuitar:
                    _leadGuitar = instrument;
                    break;
                case InstrumentIdentity.RhythmGuitar:
                    _rhythmGuitar = instrument;
                    break;
                case InstrumentIdentity.CoopGuitar:
                    _coopGuitar = instrument;
                    break;
                case InstrumentIdentity.Bass:
                    _bass = instrument;
                    break;
                case InstrumentIdentity.Keys:
                    _keys = instrument;
                    break;
                default:
                    throw new UndefinedEnumException(instrument.InstrumentIdentity);
            }
        }
        public void Set(Instrument<GHLChord> instrument)
        {
            switch (instrument.InstrumentIdentity)
            {
                case InstrumentIdentity.GHLGuitar:
                    GHLGuitar = instrument;
                    break;
                case InstrumentIdentity.GHLBass:
                    GHLBass = instrument;
                    break;
                default:
                    throw new UndefinedEnumException(instrument.InstrumentIdentity);
            }
        }

        public IEnumerator<Instrument> GetEnumerator() => new Instrument?[] { Drums, GHLGuitar, GHLBass, LeadGuitar, RhythmGuitar, CoopGuitar, Bass, Keys }.NonNull().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
