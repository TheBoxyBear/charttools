namespace ChartTools.IO.Sources;

public abstract class DataSource(Stream stream) : IDisposable
{
    public string? Path { get; }
    public Stream Stream { get; } = stream;

    private readonly bool _disposeStream = false;

    public DataSource(string path, FileMode mode, FileAccess access, FileShare share) : this(new FileStream(path, mode, access, share))
    {
        Path = path;
        _disposeStream = true;
    }

    public virtual void Dispose()
    {
        if (_disposeStream)
            Stream!.Dispose();
    }
}
