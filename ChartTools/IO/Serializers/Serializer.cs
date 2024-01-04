namespace ChartTools.IO;

internal abstract class Serializer<TResult>(string header)
{
    public string Header { get; } = header;

    public abstract IEnumerable<TResult> Serialize();
    public async Task<IEnumerable<TResult>> SerializeAsync() => await Task.Run(() => Serialize().ToArray());
}

internal abstract class Serializer<TContent, TResult>(string header, TContent content) : Serializer<TResult>(header)
{
    public TContent Content { get; } = content;
}
