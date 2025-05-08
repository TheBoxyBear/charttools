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

	public GHLInstrument? GHLRhythmGuitar
	{
		get => _ghlRhythmGuitar;
		set => _ghlRhythmGuitar = value is null ? value : value with { InstrumentIdentity = GHLInstrumentIdentity.RhythmGuitar };
	}
	private GHLInstrument? _ghlRhythmGuitar;

	public GHLInstrument? GHLCoopGuitar
	{
		get => _ghlCoopGuitar;
		set => _ghlCoopGuitar = value is null ? value : value with { InstrumentIdentity = GHLInstrumentIdentity.CoopGuitar };
	}
	private GHLInstrument? _ghlCoopGuitar;

	/// <summary>
	/// Set of lead guitar tracks
	/// </summary>
	public StandardInstrument? StandardLeadGuitar
	{
		get => _standardLeadGuitar;
		set => _standardLeadGuitar = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.LeadGuitar };
	}
	private StandardInstrument? _standardLeadGuitar;

	/// <summary>
	/// Set of rhythm guitar tracks
	/// </summary>
	public StandardInstrument? StandardRhythmGuitar
	{
		get => _standardRhythmGuitar;
		set => _standardRhythmGuitar = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.RhythmGuitar };
	}
	private StandardInstrument? _standardRhythmGuitar;

	/// <summary>
	/// Set of coop guitar tracks
	/// </summary>
	public StandardInstrument? StandardCoopGuitar
	{
		get => _standardCoopGuitar;
		set => _standardCoopGuitar = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.CoopGuitar };
	}
	private StandardInstrument? _standardCoopGuitar;

	/// <summary>
	/// Set of bass tracks
	/// </summary>
	public StandardInstrument? StandardBass
	{
		get => _standardBass;
		set => _standardBass = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.Bass };
	}
	private StandardInstrument? _standardBass;

	/// <summary>
	/// Set of keyboard tracks
	/// </summary>
	public StandardInstrument? StandardKeys
	{
		get => _standardKeys;
		set => _standardKeys = value is null ? value : value with { InstrumentIdentity = StandardInstrumentIdentity.Keys };
	}
	private StandardInstrument? _standardKeys;

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
				_standardLeadGuitar = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.StandardRhythmGuitar:
				_standardRhythmGuitar = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.StandardCoopGuitar:
				_standardCoopGuitar = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.StandardBass:
				_standardBass = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.StandardKeys:
				_standardKeys = (StandardInstrument)instrument;
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
