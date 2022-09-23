using ChartTools.Special;

using System;

namespace ChartTools
{
    /// <summary>
    /// Phrase related to a track that triggers an in-game event.
    /// </summary>
    public class TrackSpecialPhrase : SpecialPhrase
    {
        /// <summary>
        /// Type of the phrase that drives the gameplay effect
        /// </summary>
        public TrackSpecialPhraseType Type
        {
            get
            {
                var typeEnum = (TrackSpecialPhraseType)TypeCode;
                return Enum.IsDefined(typeEnum) ? typeEnum : TrackSpecialPhraseType.Unknown;
            }
            set => TypeCode = value == TrackSpecialPhraseType.Unknown ? throw new ArgumentException($"{TrackSpecialPhraseType.Unknown} is not a valid explicit value.", nameof(value)) : (byte)value;
        }

        public bool IsFaceOff => Type is TrackSpecialPhraseType.Player1FaceOff or TrackSpecialPhraseType.Player2FaceOff;

        /// <summary>
        /// Creates an instance of <see cref="TrackSpecialPhrase"/>.
        /// </summary>
        /// <param name="type">Effect of the phrase</param>
        /// <inheritdoc cref="SpecialPhrase(uint, byte, uint)"/>
        public TrackSpecialPhrase(uint position, TrackSpecialPhraseType type, uint length = 0) : base(position, (byte)type, length) { }
        /// <summary>
        /// <inheritdoc cref="TrackSpecialPhrase(uint, TrackSpecialPhraseType, uint)"/>
        /// </summary>
        /// <inheritdoc cref="SpecialPhrase(uint, byte, uint)"/>
        public TrackSpecialPhrase(uint position, byte typeCode, uint length = 0) : base(position, typeCode, length) { }

        public override bool Equals(object? obj) => Equals(obj as TrackSpecialPhrase);
        public bool Equals(TrackSpecialPhrase? other) => base.Equals(other);

        public override int GetHashCode() => base.GetHashCode();
    }
}
