namespace ChartTools.IO.Configuration
{
    /// <summary>
    /// Configuration object to direct the reading of a file
    /// </summary>
    /// <inheritdoc cref="CommonConfiguration" path="/remarks"/>
    public class ReadingConfiguration : CommonConfiguration
    {
        public UnknownSectionPolicy UnknownSectionPolicy { get; set; }
    }
}
