using ChartTools.IO.Chart.Serializers;
using ChartTools.IO.Chart.Sessions;

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools.IO.Chart
{
    internal class ChartWriter
    {
        public string Path { get; }

        private readonly ChartSerializer[] serializers;
        private readonly string tempPath = System.IO.Path.GetTempFileName();
        private readonly SemaphoreSlim semaphore = new(0);

        public ChartWriter(string path, params ChartSerializer[] serializers)
        {
            Path = path;
            this.serializers = serializers;
        }

        public void Write(WritingSession session)
        {
            Task.WaitAll(serializers.Select(serializer => Task.Run(() => serializer.Serialize(session)).ContinueWith(task => WriteFileAsync(serializer, CancellationToken.None))).ToArray());

            File.Copy(tempPath, Path);
            File.Delete(tempPath);
        }

        public async Task WriteAsync(WritingSession session, CancellationToken cancellationToken)
        {
            await Task.WhenAll(serializers.Select(serializer => Task.Run(() => serializer.Serialize(session), cancellationToken).ContinueWith(task => WriteFileAsync(serializer, cancellationToken), cancellationToken)).ToArray());

            if (!cancellationToken.IsCancellationRequested)
                File.Copy(tempPath, Path);

            File.Delete(tempPath);
        }

        private async Task WriteFileAsync(ChartSerializer serializer, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            semaphore.Wait(cancellationToken);

            using StreamWriter writer = new(tempPath);

            writer.WriteLine(serializer.Header);
            writer.WriteLine("{");

            foreach (var line in serializer.Result!)
                await writer.WriteLineAsync(line);

            writer.WriteLine("}");

            semaphore.Release();
        }
    }
}
