using ChartTools.IO.Chart.Serializers;
using ChartTools.IO.Chart.Sessions;
using ChartTools.SystemExtensions.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools.IO.Chart
{
    public static partial class ChartWriter
    {
        public static WritingConfiguration DefaultConfig = new()
        {
            SoloNoStarPowerPolicy = SoloNoStarPowerPolicy.Convert,
            EventSource = TrackObjectSource.Seperate,
            StarPowerSource = TrackObjectSource.Seperate,
            UnsupportedModifierPolicy = UnsupportedModifierPolicy.ThrowException
        };

        /// <summary>
        /// Writes a song to a chart file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="song">Song to write</param>
        public static void WriteSong(string path, Song song, WritingConfiguration? config = default)
        {
            var writer = GetSongWriter(path, song);
            writer.Write(new(config));
        }
        public static async Task WriteSongAsybnc(string path, Song song, CancellationToken cancellationToken, WritingConfiguration? config = default)
        {
            var writer = GetSongWriter(path, song);
            await writer.WriteAsync(new(config), cancellationToken);
        }
        private static ChartFileWriter GetSongWriter(string path, Song song)
        {
            var instruments = song.GetInstruments().ToArray();
            var serializers = new List<ChartSerializer>(instruments.Length + 1);
            var removedHeaders = new List<string>();

            serializers.Add(new MetadataSerializer(song.Metadata));

            if (!song.SyncTrack.IsEmpty)
                serializers.Add(new SyncTrackSerializer(song.SyncTrack));
            else
                removedHeaders.Add(ChartFormatting.SyncTrackHeader);
            if (song.GlobalEvents.Count > 0)
                serializers.Add(new GlobalEventSerializer(song.GlobalEvents));
            else
                removedHeaders.Add(ChartFormatting.GlobalEventHeader);

            foreach (var instrument in instruments.NonNull())
            {
                var instrumentName = ChartFormatting.InstrumentHeaderNames[instrument.InstrumentIdentity];

                foreach (var track in instrument.GetTracks())
                    if (!track.IsEmpty)
                        serializers.Add(new TrackSerializer(track));
                    else
                        removedHeaders.Add(ChartFormatting.Header(instrumentName, track.Difficulty));
            }

            return new(path, removedHeaders, serializers.ToArray());
        }

        /// <summary>
        /// Replaces an instrument in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <exception cref="ArgumentNullException"/>
        /// <inheritdoc cref="ReplacePart(string, IEnumerable{string}, string)" path="/exception"/>
        public static void ReplaceInstrument(string path, Instrument instrument, WritingConfiguration? config = default)
        {
            var writer = GetInstrumentWriter(path, instrument);
            writer.Write(new(config));
        }
        public static async Task ReplaceInstrumentAsync(string path, Instrument instrument, CancellationToken cancellationToken, WritingConfiguration? config = default)
        {
            var writer = GetInstrumentWriter(path, instrument);
            await writer.WriteAsync(new(config), cancellationToken);
        }
        private static ChartFileWriter GetInstrumentWriter(string path, Instrument instrument)
        {
            var instrumentName = ChartFormatting.InstrumentHeaderNames[instrument.InstrumentIdentity];
            var tracks = instrument.GetTracks().ToArray();

            return new(path, tracks.Where(t => t is null).Select(t => ChartFormatting.Header(instrumentName, t.Difficulty)), tracks.NonNull().Select(t => new TrackSerializer(t)).ToArray());
        }

        public static void ReplaceTrack(string path, Track track, WritingConfiguration? config = default)
        {
            var writer = GetTrackWriter(path, track);
            writer.Write(new(config));
        }
        public static async Task ReplaceTrackAsync(string path, Track track, CancellationToken cancellationToken, WritingConfiguration? config = default)
        {
            var writer = GetTrackWriter(path, track);
            await writer.WriteAsync(new(config), cancellationToken);
        }
        private static ChartFileWriter GetTrackWriter(string path, Track track) => new(path, null, new TrackSerializer(track));

        /// <summary>
        /// Replaces the metadata in a file.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        /// <param name="metadata">Metadata to write</param>
        /// <inheritdoc cref="ReplacePart(string, IEnumerable{string}, string)" path="/exception"/>
        public static void ReplaceMetadata(string path, Metadata metadata)
        {
            var writer = GetMetadataWriter(path, metadata);
            writer.Write(new(DefaultConfig));
        }
        public static async Task ReplaceMetadataAsync(string path, Metadata metadata, CancellationToken cancellationToken)
        {
            var writer = new ChartFileWriter(path, null, new MetadataSerializer(metadata));
            await writer.WriteAsync(new(DefaultConfig), cancellationToken);
        }
        private static ChartFileWriter GetMetadataWriter(string path, Metadata metadata) => new(path, null, new MetadataSerializer(metadata));

        /// <summary>
        /// Replaces the global events in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="events">Events to use as a replacement</param>
        /// <inheritdoc cref="ReplacePart(string, IEnumerable{string}, string)" path="/exception"/>
        public static void ReplaceGlobalEvents(string path, IEnumerable<GlobalEvent> events, WritingConfiguration? config = default)
        {
            var writer = GetGlobalEventWriter(path, events);
            writer.Write(new(config));
        }
        public static async Task ReplaceGlobalEventsAsync(string path, IEnumerable<GlobalEvent> events, CancellationToken cancellationToken, WritingConfiguration? config = default)
        {
            var writer = GetGlobalEventWriter(path, events);
            await writer.WriteAsync(new(config), cancellationToken);
        }
        private static ChartFileWriter GetGlobalEventWriter(string path, IEnumerable<GlobalEvent> events) => new(path, null, new GlobalEventSerializer(events));

        /// <summary>
        /// Replaces the sync track in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="syncTrack">Sync track to write</param>
        /// <inheritdoc cref="ReplacePart(string, IEnumerable{string}, string)" path="/exception"/>
        public static void ReplaceSyncTrack(string path, SyncTrack syncTrack, WritingConfiguration? config = default)
        {
            var writer = GetSyncTrackWriter(path, syncTrack);
            writer.Write(new(config));
        }
        public static async Task ReplaceSyncTrackAsync(string path, SyncTrack syncTrack, CancellationToken cancellationToken, WritingConfiguration config = default)
        {
            var writer = GetSyncTrackWriter(path, syncTrack);
            await writer.WriteAsync(new(config), cancellationToken);
        }
        private static ChartFileWriter GetSyncTrackWriter(string path, SyncTrack syncTrack) => new(path, null, new SyncTrackSerializer(syncTrack));
    }
}