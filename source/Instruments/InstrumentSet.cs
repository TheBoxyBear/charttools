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
    public Drums? Drums { get; set; }
    /// <summary>
    /// Set of Guitar Hero Live guitar tracks
    /// </summary>
    public GHLInstrument? GHLGuitar
    {
        get => _ghlGuitar;
        set => _ghlGuitar = value is null ? value : value with { InstrumentIdentity = GHLInstrumentIdentity.Guitar };
    }
    private GHLInstrument? _ghlGuitar;
    /// <summary>
    /// Set of Guitar Hero Live bass tracks
    /// </summary>
    public GHLInstrument? GHLBass
    {
        get => _ghlBass;
        set => _ghlBass = value is null ? value : value with { InstrumentIdentity = GHLInstrumentIdentity.Bass };
    }
    private GHLInstrument? _ghlBass;
    /// <summary>
    /// Set of lead guitar tracks
    /// </summary>
    public StandardInstrument? LeadGuitar
    {
        get => _leadGuitar;
        set => _leadGuitar = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.LeadGuitar };
    }
    private StandardInstrument? _leadGuitar;
    /// <summary>
    /// Set of rhythm guitar tracks
    /// </summary>
    public StandardInstrument? RhythmGuitar
    {
        get => _rhythmGuitar;
        set => _rhythmGuitar = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.RhythmGuitar };
    }
    private StandardInstrument? _rhythmGuitar;
    /// <summary>
    /// Set of coop guitar tracks
    /// </summary>
    public StandardInstrument? CoopGuitar
    {
        get => _coopGuitar;
        set => _coopGuitar = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.CoopGuitar };
    }
    private StandardInstrument? _coopGuitar;
    /// <summary>
    /// Set of bass tracks
    /// </summary>
    public StandardInstrument? Bass
    {
        get => _bass;
        set => _bass = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.Bass };
    }
    private StandardInstrument? _bass;
    /// <summary>
    /// Set of keyboard tracks
    /// </summary>
    public StandardInstrument? Keys
    {
        get => _keys;
        set => _keys = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.Keys };
    }
    private StandardInstrument? _keys;
    public Vocals? Vocals { get; set; }

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
        _ => throw new UndefinedEnumException(instrument)
    };
    /// <summary>
    /// Gets property value for an <see cref="Instrument{TChord}"/> from a <see cref="GHLInstrumentIdentity"/> <see langword="enum"/> value.
    /// </summary>
    /// /// <param name="instrument">Instrument to get</param>
    /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="GHLChord"/> from the <see cref="Song"/>.</returns>
    public GHLInstrument? Get(GHLInstrumentIdentity instrument) => (GHLInstrument)Get((InstrumentIdentity)instrument)!;
    /// <summary>
    /// Gets property value for an <see cref="Instrument{TChord}"/> from a <see cref="StandardInstrumentIdentity"/> <see langword="enum"/> value.
    /// </summary>
    /// <param name="instrument">Instrument to get</param>
    /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="StandardChord"/> from the <see cref="Song"/>.</returns>
    public StandardInstrument? Get(StandardInstrumentIdentity instrument) => (StandardInstrument)Get((InstrumentIdentity)instrument)!;

    public IEnumerable<Instrument> Existing() => this.NonNull().Where(instrument => !instrument.IsEmpty);

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

    public IEnumerator<Instrument> GetEnumerator() => new Instrument?[] { Drums, GHLGuitar, GHLBass, LeadGuitar, RhythmGuitar, CoopGuitar, Bass, Keys }.NonNull().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
