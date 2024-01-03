namespace ChartTools.IO
{
    /// <summary>
    /// Line of text file data
    /// </summary>
    internal readonly struct TextEntry
    {
        /// <summary>
        /// Text before the equal sign
        /// </summary>
        public string Key { get; }
        /// <summary>
        /// Text after the equal sign
        /// </summary>
        public string? Value { get; }

        public TextEntry(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public TextEntry(string line)
        {
            string[] split = line.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length < 1)
                throw new EntryException();

            Key = split[0].Trim();
            Value = split.Length < 2 ? null : split[1].Trim();
        }
    }
}
