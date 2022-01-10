using ChartTools.Internal;
using ChartTools.IO.Chart.Serializers;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.SystemExtensions;
using ChartTools.SystemExtensions.Linq;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools.IO.Chart
{
    internal class ChartFileWriter
    {
        public string Path { get; }

        private readonly List<ChartSerializer> serializers;
        private readonly string tempPath = System.IO.Path.GetTempFileName();
        private readonly IEnumerable<string>? removedHeaders;

        public ChartFileWriter(string path, IEnumerable<string>? removedHeaders, params ChartSerializer[] serializers)
        {
            Path = path;
            this.serializers = serializers.ToList();
            this.removedHeaders = removedHeaders;
        }

        private IEnumerable<SectionReplacement<string>> AddRemoveReplacements(IEnumerable<SectionReplacement<string>> replacements) => removedHeaders is null ? replacements : replacements.Concat(removedHeaders.Select(header => new SectionReplacement<string>(() => Enumerable.Empty<string>(), line => line == header, ChartFormatting.IsSectionEnd)));

        public void Write()
        {
            var replacements = AddRemoveReplacements(serializers.Select(serializer => new SectionReplacement<string>(() => serializer.Serialize(), line => line == serializer.Header, ChartFormatting.IsSectionEnd)));
            using var writer = new StreamWriter(tempPath);

            foreach (var line in File.ReadLines(Path).ReplaceSections(true, replacements))
                writer.WriteLine(line);

            File.Copy(tempPath, Path);
            File.Delete(tempPath);
        }

        public async Task WriteAsync(CancellationToken cancellationToken)
        {
            (ChartSerializer serialzier, Task<IEnumerable<string>> task)[] serializerTaskGroups = serializers.Select(serializer => (serializer, serializer.SerializeAsync())).ToArray();
            var replacements = AddRemoveReplacements(serializerTaskGroups.Select(group => new SectionReplacement<string>(() => group.task.SyncResult(), line => line == group.serialzier.Header, ChartFormatting.IsSectionEnd)));

            using var writer = new StreamWriter(tempPath);

            foreach (var line in AsyncFileReader.ReadFileAsync(tempPath).ToEnumerable().ReplaceSections(true, replacements))
                await writer.WriteLineAsync(line);

            if (!cancellationToken.IsCancellationRequested)
                File.Copy(tempPath, Path);

            File.Delete(tempPath);
        }
    }
}
