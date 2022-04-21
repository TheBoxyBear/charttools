using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools.IO
{
    internal abstract class FileReader<T> : IDisposable
    {
        public string Path { get; }
        public abstract IEnumerable<FileParser<T>> Parsers { get; }

        public FileReader(string path) => Path = path;

        public abstract void Dispose();

        public abstract void Read();
        public abstract Task ReadAsync(CancellationToken cancellationToken);
    }
}
