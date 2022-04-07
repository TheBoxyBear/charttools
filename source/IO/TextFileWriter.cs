using ChartTools.Internal.Collections;
using ChartTools.SystemExtensions.Linq;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools.IO
{
    internal abstract class TextFileWriter
    {
        public string Path { get; }
        protected virtual string? PreSerializerContent => null;
        protected virtual string? PostSerializerContent => null;

        private readonly List<Serializer<string>> serializers;
        private readonly string tempPath = System.IO.Path.GetTempFileName();
        private readonly IEnumerable<string>? removedHeaders;

        public TextFileWriter(string path, IEnumerable<string>? removedHeaders, params Serializer<string>[] serializers)
        {
            Path = path;
            this.serializers = serializers.ToList();
            this.removedHeaders = removedHeaders;
        }

        private IEnumerable<SectionReplacement<string>> AddRemoveReplacements(IEnumerable<SectionReplacement<string>> replacements) => removedHeaders is null ? replacements : replacements.Concat(removedHeaders.Select(header => new SectionReplacement<string>(Enumerable.Empty<string>(), line => line == header, EndReplace)));

        private IEnumerable<string> AddWrapper(IEnumerable<string> lines)
        {
            if (PreSerializerContent is not null)
                yield return PreSerializerContent;

            foreach (var line in lines)
                yield return line;

            if (PostSerializerContent is not null)
                yield return PostSerializerContent;
        }

        public void Write()
        {
            var replacements = AddRemoveReplacements(serializers.Select(serializer => new SectionReplacement<string>(AddWrapper(serializer.Serialize()), line => line == serializer.Header, EndReplace)));
            using var writer = new StreamWriter(tempPath);

            foreach (var line in File.ReadLines(Path).ReplaceSections(true, replacements))
                writer.WriteLine(line);

            File.Copy(tempPath, Path);
            File.Delete(tempPath);
        }
        public async Task WriteAsync(CancellationToken cancellationToken)
        {
            (Serializer<string> serialzier, IEnumerable<string> lines)[] serializerTaskGroups = serializers.Select(serializer => (serializer, AddWrapper(new EagerEnumerable<string>(serializer.SerializeAsync())))).ToArray();
            var replacements = AddRemoveReplacements(serializerTaskGroups.Select(group => new SectionReplacement<string>(group.lines, line => line == group.serialzier.Header, EndReplace)));

            using var writer = new StreamWriter(tempPath);

            foreach (var line in File.ReadLines(Path).ReplaceSections(true, replacements))
                await writer.WriteLineAsync(line);

            if (!cancellationToken.IsCancellationRequested)
                File.Copy(tempPath, Path);

            File.Delete(tempPath);
        }

        protected abstract bool EndReplace(string line);
    }
}
