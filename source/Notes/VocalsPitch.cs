using System;

namespace ChartTools.Lyrics
{
    /// <summary>
    /// Wrapper type for <see cref="VocalPitchValue"/> with helper properties to get the pitch and key
    /// </summary>
    public struct VocalsPitch : IEquatable<VocalsPitch>, IEquatable<VocalPitchValue>
    {
        /// <summary>
        /// Pitch value
        /// </summary>
        public VocalPitchValue Value { get; }
        /// <summary>
        /// Key excluding the octave
        /// </summary>
        public VocalsKey Key => (VocalsKey)((int)Value & 0x0F);
        /// <summary>
        /// Octave number
        /// </summary>
        public byte Octave => (byte)(((int)Value & 0xF0) >> 4);

        /// <summary>
        /// Creates a pitch from a raw pitch value.
        /// </summary>
        /// <param name="pitch"></param>
        public VocalsPitch(VocalPitchValue pitch) => Value = pitch;

        #region Equals
        /// <summary>
        /// Indicates if two pitches have the same value.
        /// </summary>
        /// <param name="other">Pitch to compare</param>
        public bool Equals(VocalsPitch other) => Value == other.Value;
        /// <summary>
        /// Indicates if a pitch has a value equal to a raw pitch value.
        /// </summary>
        /// <param name="other">Value to compare</param>
        public bool Equals(VocalPitchValue other) => Value == other;
        /// <summary>
        /// Indicates if an object is a raw pitch value or wrapper and the value is equal.
        /// </summary>
        /// <param name="obj">Source of value</param>
        public override bool Equals(object? obj) => obj is VocalPitchValue raw && Equals(raw) || obj is VocalsPitch wrapper && Equals(wrapper);
        #endregion

        #region Operators
        /// <summary>
        /// Converts a raw pitch value to a matching wrapper.
        /// </summary>
        /// <param name="pitch">Pitch value</param>
        public static implicit operator VocalsPitch(VocalPitchValue pitch) => new(pitch);

        /// <inheritdoc cref="Equals(VocalsPitch)"/>
        public static bool operator ==(VocalsPitch left, VocalsPitch right) => left.Equals(right);
        /// <summary>
        /// Indicates if two pitches don't have the same value.
        /// </summary>
        public static bool operator !=(VocalsPitch left, VocalsPitch right) => !(left == right);
        /// <summary>
        /// Indicates if the left pitch has a lower value than the right pitch according to music theory.
        /// </summary>
        public static bool operator <(VocalsPitch left, VocalsPitch right) => left.Value < right.Value;
        /// <summary>
        /// Indicates if the left pitch has a higher value than the right pitch according to music theory.
        /// </summary>
        public static bool operator >(VocalsPitch left, VocalsPitch right) => left.Value > right.Value;
        #endregion

        /// <summary>
        /// Returns the hash code for the pitch value.
        /// </summary>
        public override int GetHashCode() => (int)Value;
    }
}
