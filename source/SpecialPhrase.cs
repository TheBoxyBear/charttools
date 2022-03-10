using System;

namespace ChartTools
{
    /// <summary>
    /// Sequence of chords that gives star power if all the contained chords are played successfully
    /// </summary>
    public class SpecialPhrase : LongTrackObject
    {
        /// <summary>
        /// Type of the phrase that drives the gameplay effect
        /// </summary>
        public SpecialPhraseType Type
        {
            get
            {
                var typeEnum = (SpecialPhraseType)TypeCode;
                return Enum.IsDefined(typeEnum) ? typeEnum : SpecialPhraseType.Unknown;
            }
            set => TypeCode = value == SpecialPhraseType.Unknown ? throw new ArgumentException($"{SpecialPhraseType.Unknown} is not a valid explicit value.", nameof(value)) : (byte)value;
        }
        /// <summary>
        /// Numerical value of the phrase type
        /// </summary>
        public byte TypeCode { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="SpecialPhrase"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="TrackObject.Position"/></param>
        /// <param name="length">Value of <see cref="LongTrackObject.Length"/></param>
        public SpecialPhrase(uint position, SpecialPhraseType type, uint length = 0) : base(position)
        {
            Type = type;
            Length = length;
        }
        public SpecialPhrase(uint position, byte typeCode, uint length = 0) : base(position)
        {
            TypeCode = typeCode;
            Length = length;
        }

        public override bool Equals(object? obj) => Equals(obj as SpecialPhrase);
        public bool Equals(SpecialPhrase? other) => base.Equals(other);

        public override int GetHashCode() => base.GetHashCode();
    }
}
