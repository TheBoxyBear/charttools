using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;

namespace ChartTools.IO
{
    internal abstract class GroupSerializer<TContent, TResult, TProviderResult> : Serializer<TContent, TResult>
    {
        public GroupSerializer(string header, TContent content, WritingSession session) : base(header, content, session) { }

        protected abstract IEnumerable<TProviderResult>[] LaunchProviders();
        protected abstract IEnumerable<TResult> CombineProviderResults(IEnumerable<TProviderResult>[] results);

        public override IEnumerable<TResult> Serialize() => CombineProviderResults(LaunchProviders());
    }
}
