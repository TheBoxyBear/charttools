using ChartTools.IO.Configuration.Sessions;
using System.Collections.Generic;

namespace ChartTools.IO.Serializaiton;

internal abstract class GroupSerializer<TContent, TResult, TMapperResult> : Serializer<TContent, TResult>
{
    public GroupSerializer(string header, TContent content, WritingSession session) : base(header, content, session) { }

    protected abstract IEnumerable<TMapperResult>[] LaunchMappers();
    protected abstract IEnumerable<TResult> CombineMapperResults(IEnumerable<TMapperResult>[] results);

    public override IEnumerable<TResult> Serialize() => CombineMapperResults(LaunchMappers());
}
