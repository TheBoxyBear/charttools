namespace ChartTools
{
    public struct UnidentifiedMetadata
    {
        public string Key { get; init; }
        public string Value { get; set; }
        public FileType Origin { get; set; }
    }
}
