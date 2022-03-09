namespace ChartTools.IO
{
    internal static class ValueParser
    {
        public delegate bool TryParse<T>(string input, out T result);
        public static T Parse<T>(string value, string target, TryParse<T> tryParse) => tryParse(value, out T result) ? result : throw new ParseException(value, target, typeof(T));
    }
}
