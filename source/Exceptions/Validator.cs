using System;

namespace ChartTools
{
    internal static class Validator
    {
        /// <summary>
        /// Validates that an <see cref="Enum"/> value is defined.
        /// </summary>
        /// <exception cref="UndefinedEnumException"></exception>
        public static void ValidateEnum(Enum value)
        {
            if (!Enum.IsDefined(value.GetType(), value))
                throw new UndefinedEnumException(value);
        }
    }
}
