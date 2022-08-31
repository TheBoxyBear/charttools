using System;

namespace ChartTools.Lyrics
{
    /// <summary>
    /// Note pitch for a <see cref="Syllable"/>
    /// </summary>
    /// <remarks>Wrapper type for <see cref="VocalPitchValue"/> helping in obtaining the key and octave</remarks>
    public struct VocalsPitch : IEquatable<VocalsPitch>, IEquatable<VocalPitchValue>
    {
        /// <summary>
        /// Raw pitch value
        /// </summary>
        public VocalPitchValue Pitch { get; }
        /// <summary>
        /// Key excluding the octave
        /// </summary>
        public VocalsKey Key => (VocalsKey)((int)Pitch & 0x0F);
        /// <summary>
        /// Octave number
        /// </summary>
        public byte Octave => (byte)(((int)Pitch & 0xF0) >> 4);

        /// <summary>
        /// Create a <see cref="VocalsPitch"/> from a <see cref="VocalPitchValue"/>.
        /// </summary>
        /// <param name="pitch"></param>
        public VocalsPitch(VocalPitchValue pitch) => Pitch = pitch;

        #region Equals
        public bool Equals(VocalsPitch other) => Pitch == other.Pitch;
        public bool Equals(VocalPitchValue other) => Pitch == other;
        public override bool Equals(object? obj) => obj is VocalsPitch pitch && Equals(pitch);
        #endregion

        #region Operators
        public static implicit operator VocalsPitch(VocalPitchValue pitch) => new(pitch);
        public static bool operator ==(VocalsPitch left, VocalsPitch right) => left.Equals(right);
        public static bool operator !=(VocalsPitch left, VocalsPitch right) => !(left == right);
        public static bool operator <(VocalsPitch left, VocalsPitch right) => left.Pitch < right.Pitch;
        public static bool operator >(VocalsPitch left, VocalsPitch right) => left.Pitch > right.Pitch;
        #endregion

        public override int GetHashCode() => (int)Pitch;
    }
}
