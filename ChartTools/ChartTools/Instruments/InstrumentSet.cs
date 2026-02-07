using ChartTools.Extensions.Enums;
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
		get;
		set => field = value?.InstrumentIdentity is null or GHLInstrumentIdentity.LeadGuitar ? value
			: value with { InstrumentIdentity = GHLInstrumentIdentity.LeadGuitar };
	}

	/// <summary>
	/// Set of Guitar Hero Live bass tracks
	/// </summary>
	public GHLInstrument? GHLBass
	{
		get;
		set => field = value?.InstrumentIdentity is null or GHLInstrumentIdentity.Bass ? value
			: value with { InstrumentIdentity = GHLInstrumentIdentity.Bass };
	}

	public GHLInstrument? GHLRhythmGuitar
	{
		get;
		set => field = value?.InstrumentIdentity is null or GHLInstrumentIdentity.RhythmGuitar ? value
			: value with { InstrumentIdentity = GHLInstrumentIdentity.RhythmGuitar };
	}

	public GHLInstrument? GHLCoopGuitar
	{
		get;
		set => field = value?.InstrumentIdentity is null or GHLInstrumentIdentity.CoopGuitar ? value
			: value with { InstrumentIdentity = GHLInstrumentIdentity.CoopGuitar };
	}

	/// <summary>
	/// Set of lead guitar tracks
	/// </summary>
	public StandardInstrument? StandardLeadGuitar
	{
		get;
		set => field = value?.InstrumentIdentity is null or StandardInstrumentIdentity.LeadGuitar ? value
			: value with { InstrumentIdentity = StandardInstrumentIdentity.LeadGuitar };
	}

	/// <summary>
	/// Set of rhythm guitar tracks
	/// </summary>
	public StandardInstrument? StandardRhythmGuitar
	{
		get;
		set => field = value?.InstrumentIdentity is null or StandardInstrumentIdentity.RhythmGuitar ? value
			: value with { InstrumentIdentity = StandardInstrumentIdentity.RhythmGuitar };
	}

	/// <summary>
	/// Set of coop guitar tracks
	/// </summary>
	public StandardInstrument? StandardCoopGuitar
	{
		get;
		set => field = value?.InstrumentIdentity is null or StandardInstrumentIdentity.CoopGuitar ? value
			: value with { InstrumentIdentity = StandardInstrumentIdentity.CoopGuitar };
	}

	/// <summary>
	/// Set of bass tracks
	/// </summary>
	public StandardInstrument? StandardBass
	{
		get;
		set => field = value?.InstrumentIdentity is null or StandardInstrumentIdentity.Bass ? value
			: value with { InstrumentIdentity = StandardInstrumentIdentity.Bass };
	}

	/// <summary>
	/// Set of keyboard tracks
	/// </summary>
	public StandardInstrument? StandardKeys
	{
		get;
		set => field = value?.InstrumentIdentity is null or StandardInstrumentIdentity.Keys ? value
			: value with { InstrumentIdentity = StandardInstrumentIdentity.Keys };
	}

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
		instrument.Validate();
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
		instrument.Validate();
		return Get((InstrumentIdentity)instrument) as StandardInstrument;
	}

	public IEnumerable<Instrument> Existing()
		=> this.NonNull().Where(static instrument => !instrument.IsEmpty);

	public void Set(Instrument instrument)
	{
		switch (instrument.InstrumentIdentity)
		{
			case InstrumentIdentity.Drums:
				Drums = (Drums)instrument;
				break;
			// Instruments must be set by field as calling the setter replaces the record instance
			case InstrumentIdentity.StandardLeadGuitar:
				StandardLeadGuitar   = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.StandardRhythmGuitar:
				StandardRhythmGuitar = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.StandardCoopGuitar:
				StandardCoopGuitar   = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.StandardBass:
				StandardBass         = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.StandardKeys:
				StandardKeys         = (StandardInstrument)instrument;
				break;
			case InstrumentIdentity.GHLLeadGuitar:
				GHLLeadGuitar        = (GHLInstrument)instrument;
				break;
			case InstrumentIdentity.GHLBass:
				GHLBass              = (GHLInstrument)instrument;
				break;
			case InstrumentIdentity.GHLRhythmGuitar:
				GHLRhythmGuitar      = (GHLInstrument)instrument;
				break;
			case InstrumentIdentity.GHLCoopGuitar:
				GHLCoopGuitar        = (GHLInstrument)instrument;
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
			GHLLeadGuitar, GHLBass, GHLRhythmGuitar, GHLCoopGuitar,
			StandardLeadGuitar, StandardRhythmGuitar, StandardCoopGuitar, StandardBass, StandardKeys
		}
		.NonNull().GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();
}
