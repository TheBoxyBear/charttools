using System.Collections.Generic;

namespace ChartTools.IO.Chart
{
    internal class ChartFileWriter : TextFileWriter
    {
        protected override string? PreSerializerContent => "{";
        protected override string? PostSerializerContent => "}";

        public ChartFileWriter(string path, IEnumerable<string>? removedHeaders, params Serializer<string>[] serializers) : base(path, removedHeaders, serializers) { }

        protected override bool EndReplace(string line) => line.StartsWith('[');
    }
}
