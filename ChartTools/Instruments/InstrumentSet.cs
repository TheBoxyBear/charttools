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
		get => m_ghlGuitar;
		set => m_ghlGuitar = value is null ? value : value with { InstrumentIdentity = GHLInstrumentIdentity.Guitar };
	}
	private GHLInstrument? m_ghlGuitar;

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
		get => m_leadGuitar;
		set => m_leadGuitar = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.LeadGuitar };
	}
	private StandardInstrument? m_leadGuitar;

	/// <summary>
	/// Set of rhythm guitar tracks
	/// </summary>
	public StandardInstrument? StandardRhythmGuitar
	{
		get => m_rhythmGuitar;
		set => m_rhythmGuitar = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.RhythmGuitar };
	}
	private StandardInstrument? m_rhythmGuitar;

	/// <summary>
	/// Set of coop guitar tracks
	/// </summary>
	public StandardInstrument? StandardCoopGuitar
	{
		get => m_coopGuitar;
		set => m_coopGuitar = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.CoopGuitar };
	}
	private StandardInstrument? m_coopGuitar;

	/// <summary>
	/// Set of bass tracks
	/// </summary>
	public StandardInstrument? StandardBass
	{
		get => m_bass;
		set => m_bass = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.Bass };
	}
	private StandardInstrument? m_bass;

	/// <summary>
	/// Set of keyboard tracks
	/// </summary>
	public StandardInstrument? StandardKeys
	{
		get => m_keys;
		set => m_keys = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.Keys };
	}
	private StandardInstrument? m_keys;

	/// <summary>
	/// Gets property value for an <see cref="Instrument"/> from a <see cref="InstrumentIdentity"/> <see langword="enum"/> value.
	/// </summary>
	/// <returns>Instance of <see cref="Instrument"/> from the <see cref="Song"/></returns>
	/// <param name="instrument">Instrument to get</param>
	public Instrument? Get(InstrumentIdentity instrument) => instrument switch
	{
		InstrumentIdentity.Drums                => Drums,
		InstrumentIdentity.GHLGuitar            => GHLGuitar,
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
	public StandardInstrument? Get(StandardInstrumentIdentity instrument)
	{
		Validator.ValidateEnum(instrument);
		return Get((InstrumentIdentity)instrument) as StandardInstrument;
	}

	public IEnumerable<Instrument> Existing() => this.NonNull().Where(instrument => !instrument.IsEmpty);

	public void Set(Instrument instrument)
	{
		switch (instrument.InstrumentIdentity)
		{
			case InstrumentIdentity.Drums:
				Drums = (Drums)instrument;
				break;
			case InstrumentIdentity.StandardLeadGuitar:
				StandardLeadGuitar = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.StandardRhythmGuitar:
				StandardRhythmGuitar = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.StandardCoopGuitar:
				StandardCoopGuitar = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.StandardBass:
				StandardBass = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.StandardKeys:
				StandardKeys = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.GHLGuitar:
				GHLGuitar = (GHLInstrument)instrument;
				break;
			case InstrumentIdentity.GHLBass:
				GHLBass = (GHLInstrument)instrument;
				break;
			case InstrumentIdentity.GHLRhythmGuitar:
				GHLRhythmGuitar = (GHLInstrument)instrument;
				break;
			case InstrumentIdentity.GHLCoopGuitar:
				GHLCoopGuitar = (GHLInstrument)instrument;
				break;
			default:
				throw new UndefinedEnumException(instrument.InstrumentIdentity);
		}
	}

    /// <inheritdoc cref="IEnumerable{Instrument}.GetEnumerator"/>
	public IEnumerator<Instrument> GetEnumerator()
		=> new Instrument?[]
		{ Drums, GHLGuitar, GHLBass, GHLRhythmGuitar, GHLCoopGuitar, StandardLeadGuitar, StandardRhythmGuitar, StandardCoopGuitar, StandardBass, StandardKeys }
		.NonNull().GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
