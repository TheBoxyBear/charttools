using ChartTools.IO.Formatting;
using ChartTools.IO.Ini;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools.IO
{
    public record DirectoryResult<T>(T Result, Metadata Metadata);

    internal static class DirectoryHandler
    {
        public static DirectoryResult<T?> FromDirectory<T>(string directory, Func<string, FormattingRules, T> read)
        {
            var iniPath = directory + @"\song.ini";
            var chartPath = directory + @"\notes.chart";
            var iniMetadata = File.Exists(iniPath) ? IniFile.ReadMetadata(iniPath) : new();

            T? value = default;

            if (File.Exists(chartPath))
                value = read(chartPath, iniMetadata.Formatting);

            return new(value, iniMetadata);
        }
        public static async Task<DirectoryResult<T?>> FromDirectoryAsync<T>(string directory, Func<string, FormattingRules, Task<T>> read, CancellationToken cancellationToken)
        {
            var iniPath = directory + @"\song.ini";
            var chartPath = directory + @"\notes.chart";
            var iniMetadata = File.Exists(iniPath) ? await IniFile.ReadMetadataAsync(iniPath, null, cancellationToken) : new();

            T? value = default;

            if (File.Exists(chartPath))
                value = await read(chartPath, iniMetadata.Formatting);

            return new(value, iniMetadata);
        }
    }
}
