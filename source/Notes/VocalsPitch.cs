using ChartTools.Internal.Collections;

using System;

namespace ChartTools.Lyrics
{
    /// <summary>
    /// Wrapper type for <see cref="VocalsPitches"/> with helper properties to get the pitch and key
    /// </summary>
    public struct VocalsPitch : IEquatable<VocalsPitch>, IEquatable<VocalsPitches>
    {
        /// <summary>
        /// Pitch value
        /// </summary>
        public VocalsPitches Pitch { get; }
        /// <summary>
        /// Key excluding the octave
        /// </summary>
        public VocalsKey Key => (VocalsKey)((int)Pitch & 0x0F);
        /// <summary>
        /// Octave number
        /// </summary>
        public byte Octave => (byte)(((int)Pitch & 0xF0) >> 4);

        /// <summary>
        /// Creates a pitch from a raw pitch value.
        /// </summary>
        /// <param name="pitch"></param>
        public VocalsPitch(VocalsPitches pitch) => Pitch = pitch;

        #region Equals
        /// <summary>
        /// Indicates if two pitches have the same value.
        /// </summary>
        /// <param name="other">Pitch to compare</param>
        public bool Equals(VocalsPitch other) => Pitch == other.Pitch;
        /// <summary>
        /// Indicates if a pitch has a value equal to a raw pitch value.
        /// </summary>
        /// <param name="other">Value to compare</param>
        public bool Equals(VocalsPitches other) => Pitch == other;
        /// <summary>
        /// Indicates if an object is a raw pitch value or wrapper and the value is equal.
        /// </summary>
        /// <param name="obj">Source of value</param>
        public override bool Equals(object? obj) => obj is VocalsPitches raw && Equals(raw) || obj is VocalsPitch wrapper && Equals(wrapper);
        #endregion

        #region Operators
        /// <summary>
        /// Converts a raw pitch value to a matching wrapper.
        /// </summary>
        /// <param name="pitch">Pitch value</param>
        public static implicit operator VocalsPitch(VocalsPitches pitch) => new(pitch);

        /// <inheritdoc cref="Equals(VocalsPitch)"/>
        public static bool operator ==(VocalsPitch left, VocalsPitch right) => left.Equals(right);
        /// <summary>
        /// Indicates if two pitches don't have the same value.
        /// </summary>
        public static bool operator !=(VocalsPitch left, VocalsPitch right) => !(left == right);
        /// <summary>
        /// Indicates if the left pitch has a lower value than the right pitch according to music theory.
        /// </summary>
        public static bool operator <(VocalsPitch left, VocalsPitch right) => left.Pitch < right.Pitch;
        /// <summary>
        /// Indicates if the left pitch has a higher value than the right pitch according to music theory.
        /// </summary>
        public static bool operator >(VocalsPitch left, VocalsPitch right) => left.Pitch > right.Pitch;
        #endregion

        /// <summary>
        /// Returns the hash code for the pitch value.
        /// </summary>
        public override int GetHashCode() => (int)Pitch;
    }
}
