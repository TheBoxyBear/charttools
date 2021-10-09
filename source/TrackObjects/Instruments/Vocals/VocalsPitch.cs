using System;

namespace ChartTools
{
    public struct VocalsPitch : IEquatable<VocalsPitch>, IEquatable<VocalsPitches>
    {
        VocalsPitches _pitch = VocalsPitches.C2;
        public VocalsPitches Pitch
        {
            get => _pitch;
            set => _pitch = Enum.IsDefined(value) ? value : throw CommonExceptions.GetUndefinedException(value);
        }

        public VocalsPitch(VocalsPitches pitch) => Pitch = pitch;

        public VocalsKey Key
        {
            get => (VocalsKey)((int)Pitch & 0x0F);
            set
            {
                if (!Enum.IsDefined(value))
                    throw CommonExceptions.GetUndefinedException(value);

                int unadjustedOctave = UnadjustedOctave;

                if (unadjustedOctave is 0x20 or 0x60 && value != VocalsKey.C)
                    throw new ArgumentException($"Only C can be used with octave {Octave}", nameof(value));

                Pitch = (VocalsPitches)(UnadjustedOctave | (int)value);
            }
        }
        public byte Octave
        {
            get => (byte)(UnadjustedOctave >> 4);
            set
            {
                if (value is < 2 or > 6)
                    throw new ArgumentException("The octave must be in the range 2-6.", nameof(value));
                if (value is 2 or 6 && Key != VocalsKey.C)
                    throw new ArgumentException($"Octave {value} can only be used with C.", nameof(value));

                Pitch = (VocalsPitches)((value << 4) | (int)Key);
            }
        }
        private int UnadjustedOctave => (int)Pitch & 0xF0;

        #region Equals
        public bool Equals(VocalsPitch other) => Pitch == other.Pitch;
        public bool Equals(VocalsPitches other) => Pitch == other;
        public override bool Equals(object? obj) => obj is VocalsPitch pitch && Equals(pitch);
        #endregion

        #region Operators
        public static implicit operator VocalsPitch(VocalsPitches pitch) => new(pitch);
        public static bool operator ==(VocalsPitch left, VocalsPitch right) => left.Equals(right);
        public static bool operator !=(VocalsPitch left, VocalsPitch right) => !(left == right);
        public static bool operator <(VocalsPitch left, VocalsPitch right) => left.Pitch < right.Pitch;
        public static bool operator >(VocalsPitch left, VocalsPitch right) => left.Pitch > right.Pitch;
        #endregion
    }
}
