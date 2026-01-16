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
	public GHLInstrument? GHLLeadGuitar
	{
		get => m_ghlLeadGuitar;
		set => m_ghlLeadGuitar = value is null ? value : value with { InstrumentIdentity = GHLInstrumentIdentity.LeadGuitar };
	}
	private GHLInstrument? m_ghlLeadGuitar;

	/// <summary>
	/// Set of Guitar Hero Live bass tracks
	/// </summary>
	public GHLInstrument? GHLBass
	{
		get => m_ghlBass;
		set => m_ghlBass = value is null ? value : value with { InstrumentIdentity = GHLInstrumentIdentity.Bass };
	}
	private GHLInstrument? m_ghlBass;

	public GHLInstrument? GHLRhythmGuitar
	{
		get => m_ghlRhythmGuitar;
		set => m_ghlRhythmGuitar = value is null ? value : value with { InstrumentIdentity = GHLInstrumentIdentity.RhythmGuitar };
	}
	private GHLInstrument? m_ghlRhythmGuitar;

	public GHLInstrument? GHLCoopGuitar
	{
		get => m_ghlCoopGuitar;
		set => m_ghlCoopGuitar = value is null ? value : value with { InstrumentIdentity = GHLInstrumentIdentity.CoopGuitar };
	}
	private GHLInstrument? m_ghlCoopGuitar;

	/// <summary>
	/// Set of lead guitar tracks
	/// </summary>
	public StandardInstrument? StandardLeadGuitar
	{
		get => m_standardLeadGuitar;
		set => m_standardLeadGuitar = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.LeadGuitar };
	}
	private StandardInstrument? m_standardLeadGuitar;

	/// <summary>
	/// Set of rhythm guitar tracks
	/// </summary>
	public StandardInstrument? StandardRhythmGuitar
	{
		get => m_standardRhythmGuitar;
		set => m_standardRhythmGuitar = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.RhythmGuitar };
	}
	private StandardInstrument? m_standardRhythmGuitar;

	/// <summary>
	/// Set of coop guitar tracks
	/// </summary>
	public StandardInstrument? StandardCoopGuitar
	{
		get => m_standardCoopGuitar;
		set => m_standardCoopGuitar = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.CoopGuitar };
	}
	private StandardInstrument? m_standardCoopGuitar;

	/// <summary>
	/// Set of bass tracks
	/// </summary>
	public StandardInstrument? StandardBass
	{
		get => m_standardBass;
		set => m_standardBass = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.Bass };
	}
	private StandardInstrument? m_standardBass;

	/// <summary>
	/// Set of keyboard tracks
	/// </summary>
	public StandardInstrument? StandardKeys
	{
		get => m_standardKeys;
		set => m_standardKeys = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.Keys };
	}
	private StandardInstrument? m_standardKeys;

	/// <summary>
	/// Gets property value for an <see cref="Instrument"/> from a <see cref="InstrumentIdentity"/> <see langword="enum"/> value.
	/// </summary>
	/// <returns>Instance of <see cref="Instrument"/> from the <see cref="Song"/></returns>
	/// <param name="instrument">Instrument to get</param>
	public Instrument? Get(InstrumentIdentity instrument) => instrument switch
	{
		InstrumentIdentity.Drums                => Drums,
		InstrumentIdentity.GHLLeadGuitar        => GHLLeadGuitar,
		InstrumentIdentity.GHLBass              => GHLBass,
		InstrumentIdentity.GHLRhythmGuitar      => GHLRhythmGuitar,
		InstrumentIdentity.GHLCoopGuitar        => GHLCoopGuitar,
		InstrumentIdentity.StandardLeadGuitar   => StandardLeadGuitar,
		InstrumentIdentity.StandardRhythmGuitar => StandardRhythmGuitar,
		InstrumentIdentity.StandardCoopGuitar   => StandardCoopGuitar,
		InstrumentIdentity.StandardBass         => StandardBass,
		InstrumentIdentity.StandardKeys         => StandardKeys,
		_ => throw new UndefinedEnumException(instrument)
	};

	/// <summary>
	/// Gets property value for a <see cref="GHLInstrument"/> from a <see cref="GHLInstrumentIdentity"/> <see langword="enum"/> value.
	/// </summary>
	/// /// <param name="instrument">Instrument to get</param>
	/// <returns>Instance of <see cref="GHLInstrument"/> from the <see cref="Song"/>.</returns>
	/// <exception cref="UndefinedEnumException"/>
	public GHLInstrument? Get(GHLInstrumentIdentity instrument)
	{
		Validator.ValidateEnum(instrument);
		return Get((InstrumentIdentity)instrument) as GHLInstrument;
	}

	/// <summary>
	/// Gets property value for a <see cref="StandardInstrument"/> from a <see cref="StandardInstrumentIdentity"/> <see langword="enum"/> value.
	/// </summary>
	/// <param name="instrument">Instrument to get</param>
	/// <returns>Instance of <see cref="StandardInstrument"/> from the <see cref="Song"/>.</returns>
	/// <exception cref="UndefinedEnumException"/>
	public StandardInstrument? Get(StandardInstrumentIdentity instrument)
	{
		Validator.ValidateEnum(instrument);
		return Get((InstrumentIdentity)instrument) as StandardInstrument;
	}

	public IEnumerable<Instrument> Existing()
		=> this.NonNull().Where(instrument => !instrument.IsEmpty);

	public void Set(Instrument instrument)
	{
		switch (instrument.InstrumentIdentity)
		{
			case InstrumentIdentity.Drums:
				Drums = (Drums)instrument;
				break;
			// Instruments must be set by field as calling the setter replaces the record instance
			case InstrumentIdentity.StandardLeadGuitar:
				m_standardLeadGuitar   = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.StandardRhythmGuitar:
				m_standardRhythmGuitar = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.StandardCoopGuitar:
				m_standardCoopGuitar   = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.StandardBass:
				m_standardBass         = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.StandardKeys:
				m_standardKeys         = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.GHLLeadGuitar:
				m_ghlLeadGuitar        = (GHLInstrument)instrument;
				break;
			case InstrumentIdentity.GHLBass:
				m_ghlBass              = (GHLInstrument)instrument;
				break;
			case InstrumentIdentity.GHLRhythmGuitar:
				m_ghlRhythmGuitar      = (GHLInstrument)instrument;
				break;
			case InstrumentIdentity.GHLCoopGuitar:
				m_ghlCoopGuitar        = (GHLInstrument)instrument;
				break;
			default:
				throw new UndefinedEnumException(instrument.InstrumentIdentity);
		}
	}

	/// <inheritdoc cref="IEnumerable{Instrument}.GetEnumerator"/>
	public IEnumerator<Instrument> GetEnumerator()
		=> new Instrument?[]
		{
			Drums,
			GHLLeadGuitar,
			GHLBass,
			GHLRhythmGuitar,
			GHLCoopGuitar,
			StandardLeadGuitar,
			StandardRhythmGuitar,
			StandardCoopGuitar,
			StandardBass,
			StandardKeys
		}
		.NonNull().GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();
}
