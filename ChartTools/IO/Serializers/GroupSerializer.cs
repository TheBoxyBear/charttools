namespace ChartTools.IO;

internal abstract class GroupSerializer<TContent, TResult, TProviderResult>(string header, TContent content)
    : Serializer<TContent, TResult>(header, content)
{
    protected abstract IEnumerable<TProviderResult>[] LaunchProviders();
    protected abstract IEnumerable<TResult> CombineProviderResults(IEnumerable<TProviderResult>[] results);

    public override IEnumerable<TResult> Serialize() => CombineProviderResults(LaunchProviders());
}
