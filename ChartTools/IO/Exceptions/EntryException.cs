namespace ChartTools.IO;

public class EntryException : FormatException
{
    public EntryException() : base("Cannot divide line into entry elements.") { }
}
