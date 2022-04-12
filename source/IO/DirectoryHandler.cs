using ChartTools.Formatting;
using ChartTools.IO.Ini;

using System;
using System.IO;
using System.Threading.Tasks;

namespace ChartTools.IO
{
    public record DirectoryResult<T>(T Result, Metadata Metadata);

    internal static class DirectoryHandler
    {
        public static DirectoryResult<T?> FromDirectory<T>(string directory, Func<string, FormattingRules, T> read)
        {
            (var path, Metadata metadata) = Base(directory);
            T? value = default;

            if (File.Exists(path))
                value = read(path, metadata.Formatting);

            return new(value, metadata);
        }
        public static async Task<DirectoryResult<T?>> FromDirectoryAsync<T>(string directory, Func<string, FormattingRules, Task<T>> read)
        {
            (var path, Metadata metadata) = Base(directory);
            T? value = default;

            if (File.Exists(path))
                value = await read(path, metadata.Formatting);

            return new(value, metadata);
        }

        private static DirectoryResult<string> Base(string directory)
        {
            var iniPath = directory + "song.ini";
            var chartPath = directory + "notes.chart";
            var iniMetadata = File.Exists(iniPath) ? IniFile.ReadMetadata(iniPath) : new();

            return new(chartPath, iniMetadata);
        }
    }
}
