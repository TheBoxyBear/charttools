using System.Collections.Generic;
using System.IO;

namespace ChartTools.Internal
{
    public static class AsyncFileReader
    {
        public static async IAsyncEnumerable<string> ReadFileAsync(string path)
        {
            using StreamReader reader = new(path);
            string? line = null;

            do
            {
                var readTask = reader.ReadLineAsync();

                if (!string.IsNullOrEmpty(line))
                    yield return line;

                line = (await readTask)?.Trim();
            }
            while (!reader.EndOfStream);

            yield return line!;
        }
    }
}
