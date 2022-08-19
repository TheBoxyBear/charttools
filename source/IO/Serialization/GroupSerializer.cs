using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;

namespace ChartTools.IO.Serializaiton
{
    internal abstract class GroupSerializer<TContent, TResult, TProviderResult> : Serializer<TContent, TResult>
    {
        public GroupSerializer(string header, TContent content, WritingSession session) : base(header, content, session) { }

        protected abstract IEnumerable<TProviderResult>[] LaunchMappers();
        protected abstract IEnumerable<TResult> CombineMapperResults(IEnumerable<TProviderResult>[] results);

        public override IEnumerable<TResult> Serialize() => CombineMapperResults(LaunchMappers());
    }
}
