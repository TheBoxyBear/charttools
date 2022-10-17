using System;

namespace ChartTools
{
    internal static class Validator
    {
        /// <summary>
        /// Validates that an <see cref="Enum"/> value is defined.
        /// </summary>
        /// <param name="value">Value to validate</param>
        /// <exception cref="UndefinedEnumException"></exception>
        public static void ValidateEnum(Enum value)
        {
            if (!Enum.IsDefined(value.GetType(), value))
                throw new UndefinedEnumException(value);
        }
        /// <summary>
        /// Validates that a track is attached to an instrument.
        /// </summary>
        /// <param name="track">Track to validate</param>
        public static void ValidateParentInstrument(Track track)
        {
            if (track.ParentInstrument is null)
                throw new DetachedTrackException(track);
        }
    }
}
