using ChartTools.Extensions.Linq;

using System.Collections;

namespace ChartTools;

/// <summary>
/// Set of all instruments
/// </summary>
public class InstrumentSet : IEnumerable<Instrument>
{
    /// <summary>
    /// Set of drums tracks
    /// </summary>
    public Drums Drums { get; set; } = new();
    /// <summary>
    /// Set of Guitar Hero Live guitar tracks
    /// </summary>
    public GHLInstrument GHLGuitar
    {
        get => _ghlGuitar;
        set => _ghlGuitar = value with { InstrumentIdentity = GHLInstrumentIdentity.Guitar };
    }
    private GHLInstrument _ghlGuitar = new(GHLInstrumentIdentity.Guitar);
    /// <summary>
    /// Set of Guitar Hero Live bass tracks
    /// </summary>
    public GHLInstrument GHLBass
    {
        get => _ghlBass;
        set => _ghlBass = value with { InstrumentIdentity = GHLInstrumentIdentity.Bass };
    }
    private GHLInstrument _ghlBass = new(GHLInstrumentIdentity.Bass);
    /// <summary>
    /// Set of lead guitar tracks
    /// </summary>
    public StandardInstrument LeadGuitar
    {
        get => _leadGuitar;
        set => _leadGuitar = value with { InstrumentIdentity = StandardInstrumentIdentity.LeadGuitar };
    }
    private StandardInstrument _leadGuitar = new(StandardInstrumentIdentity.LeadGuitar);
    /// <summary>
    /// Set of rhythm guitar tracks
    /// </summary>
    public StandardInstrument RhythmGuitar
    {
        get => _rhythmGuitar;
        set => _rhythmGuitar = value with { InstrumentIdentity = StandardInstrumentIdentity.RhythmGuitar };
    }
    private StandardInstrument _rhythmGuitar = new(StandardInstrumentIdentity.RhythmGuitar);
    /// <summary>
    /// Set of coop guitar tracks
    /// </summary>
    public StandardInstrument CoopGuitar
    {
        get => _coopGuitar;
        set => _coopGuitar = value with { InstrumentIdentity = StandardInstrumentIdentity.CoopGuitar };
    }
    private StandardInstrument _coopGuitar = new(StandardInstrumentIdentity.CoopGuitar);
    /// <summary>
    /// Set of bass tracks
    /// </summary>
    public StandardInstrument Bass
    {
        get => _bass;
        set => _bass = value with { InstrumentIdentity = StandardInstrumentIdentity.Bass };
    }
    private StandardInstrument _bass = new(StandardInstrumentIdentity.Bass);
    /// <summary>
    /// Set of keyboard tracks
    /// </summary>
    public StandardInstrument Keys
    {
        get => _keys;
        set => _keys = value with { InstrumentIdentity = StandardInstrumentIdentity.Keys };
    }
    private StandardInstrument _keys = new(StandardInstrumentIdentity.Keys);
    public Vocals Vocals { get; set; } = new();

    /// <summary>
    /// Gets property value for an <see cref="Instrument"/> from a <see cref="InstrumentIdentity"/> <see langword="enum"/> value.
    /// </summary>
    /// <returns>Instance of <see cref="Instrument"/> from the <see cref="Song"/></returns>
    /// <param name="instrument">Instrument to get</param>
    public Instrument Get(InstrumentIdentity instrument) => instrument switch
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
        _ => throw new UndefinedEnumException(instrument)
    };
    /// <summary>
    /// Gets property value for an <see cref="Instrument{TChord}"/> from a <see cref="GHLInstrumentIdentity"/> <see langword="enum"/> value.
    /// </summary>
    /// /// <param name="instrument">Instrument to get</param>
    /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="GHLChord"/> from the <see cref="Song"/>.</returns>
    public GHLInstrument Get(GHLInstrumentIdentity instrument) => (GHLInstrument)Get((InstrumentIdentity)instrument);
    /// <summary>
    /// Gets property value for an <see cref="Instrument{TChord}"/> from a <see cref="StandardInstrumentIdentity"/> <see langword="enum"/> value.
    /// </summary>
    /// <param name="instrument">Instrument to get</param>
    /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="StandardChord"/> from the <see cref="Song"/>.</returns>
    public StandardInstrument Get(StandardInstrumentIdentity instrument) => (StandardInstrument)Get((InstrumentIdentity)instrument);

    public IEnumerable<Instrument> Existing() => this.Where(instrument => !instrument.IsEmpty);

    public void Set(StandardInstrument instrument)
    {
        switch (instrument.InstrumentIdentity)
        {
            case StandardInstrumentIdentity.LeadGuitar:
                _leadGuitar = instrument;
                break;
            case StandardInstrumentIdentity.RhythmGuitar:
                _rhythmGuitar = instrument;
                break;
            case StandardInstrumentIdentity.CoopGuitar:
                _coopGuitar = instrument;
                break;
            case StandardInstrumentIdentity.Bass:
                _bass = instrument;
                break;
            case StandardInstrumentIdentity.Keys:
                _keys = instrument;
                break;
            default:
                throw new UndefinedEnumException(instrument.InstrumentIdentity);
        }
    }
    public void Set(GHLInstrument instrument)
    {
        switch (instrument.InstrumentIdentity)
        {
            case GHLInstrumentIdentity.Guitar:
                GHLGuitar = instrument;
                break;
            case GHLInstrumentIdentity.Bass:
                GHLBass = instrument;
                break;
            default:
                throw new UndefinedEnumException(instrument.InstrumentIdentity);
        }
    }

    public IEnumerator<Instrument> GetEnumerator() => new Instrument[] { Drums, GHLGuitar, GHLBass, LeadGuitar, RhythmGuitar, CoopGuitar, Bass, Keys }.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
