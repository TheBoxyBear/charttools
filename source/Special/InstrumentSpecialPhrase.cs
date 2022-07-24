using System;

namespace ChartTools
{
    /// <summary>
    /// Phrase related to an instrument that triggers an in-game event.
    /// </summary>
    public class InstrumentSpecialPhrase : LongTrackObject
    {
        /// <summary>
        /// Type of the phrase that drives the gameplay effect
        /// </summary>
        public InstrumentSpecialPhraseType Type
        {
            get
            {
                var typeEnum = (InstrumentSpecialPhraseType)TypeCode;
                return Enum.IsDefined(typeEnum) ? typeEnum : InstrumentSpecialPhraseType.Unknown;
            }
            set => TypeCode = value == InstrumentSpecialPhraseType.Unknown ? throw new ArgumentException($"{InstrumentSpecialPhraseType.Unknown} is not a valid explicit value.", nameof(value)) : (byte)value;
        }
        /// <summary>
        /// Numerical value of the phrase type
        /// </summary>
        public byte TypeCode { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="InstrumentSpecialPhrase"/>.
        /// </summary>
        /// <param name="position">Position of the phrase</param>
        /// <param name="type">Effect of the phrase</param>
        /// <param name="length">Duration in ticks</param>
        public InstrumentSpecialPhrase(uint position, InstrumentSpecialPhraseType type, uint length = 0) : base(position)
        {
            Type = type;
            Length = length;
        }
        /// <inheritdoc cref="InstrumentSpecialPhrase(uint, InstrumentSpecialPhraseType, uint)"/>
        /// <param name="typeCode">Numeric value of the phrase type. Can be used to define custom phrases.</param>
        public InstrumentSpecialPhrase(uint position, byte typeCode, uint length = 0) : base(position)
        {
            TypeCode = typeCode;
            Length = length;
        }

        public override bool Equals(object? obj) => Equals(obj as TrackSpecialPhrase);
        public bool Equals(TrackSpecialPhrase? other) => base.Equals(other);

        public override int GetHashCode() => base.GetHashCode();
    }
}
