namespace ChartTools.IO
{
    internal static class ValueParser
    {
        public delegate bool TryParse<T>(string? input, out T result);
        public static T Parse<T>(string? value, string target, TryParse<T> tryParse) where T : struct => tryParse(value, out T result) ? result : throw new ParseException(value, target, typeof(T));

        public static bool ParseBool(string? value, string target) => Parse<bool>(value, target, bool.TryParse);
        public static byte ParseByte(string? value, string target) => Parse<byte>(value, target, byte.TryParse);
        public static sbyte ParseSbyte(string? value, string target) => Parse<sbyte>(value, target, sbyte.TryParse);
        public static short ParseShort(string? value, string target) => Parse<short>(value, target, short.TryParse);
        public static ushort ParseUshort(string? value, string target) => Parse<ushort>(value, target, ushort.TryParse);
        public static int ParseInt(string? value, string target) => Parse<int>(value, target, int.TryParse);
        public static uint ParseUint(string? value, string target) => Parse<uint>(value, target, uint.TryParse);
        public static float ParseFloat(string? value, string target) => Parse<float>(value, target, float.TryParse);
    }
}
