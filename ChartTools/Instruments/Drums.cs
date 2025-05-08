namespace ChartTools;

public record Drums : Instrument<DrumsChord>
{
	protected override InstrumentIdentity GetIdentity() => InstrumentIdentity.Drums;
}
