namespace ChartTools;

/// <summary>
/// Phrase related to an instrument that triggers an in-game event.
/// </summary>
public class InstrumentSpecialPhrase : SpecialPhrase
{
	/// <summary>
	/// Type of the phrase that drives the gameplay effect
	/// </summary>
	public InstrumentSpecialPhraseType Type
	{
		get
		{
			InstrumentSpecialPhraseType typeEnum = (InstrumentSpecialPhraseType)TypeCode;
			return Enum.IsDefined(typeEnum) ? typeEnum : InstrumentSpecialPhraseType.Unknown;
		}
		set => TypeCode = value is InstrumentSpecialPhraseType.Unknown
			? throw new ArgumentException($"{InstrumentSpecialPhraseType.Unknown} is not a valid explicit value.", nameof(value))
			: (byte)value;
	}

	/// <summary>
	/// Creates an instance of <see cref="InstrumentSpecialPhrase"/>.
	/// </summary>
	/// <param name="type">Effect of the phrase</param>
	/// <inheritdoc cref="SpecialPhrase(uint, byte, uint)"/>
	public InstrumentSpecialPhrase(uint position, InstrumentSpecialPhraseType type, uint length = 0) : base(position, (byte)type, length) { }

	/// <summary>
	/// <inheritdoc cref="InstrumentSpecialPhrase(uint, InstrumentSpecialPhraseType, uint)"/>
	/// </summary>
	/// <inheritdoc cref="SpecialPhrase(uint, byte, uint)"/>
	public InstrumentSpecialPhrase(uint position, byte typeCode, uint length = 0) : base(position, typeCode, length) { }
}
