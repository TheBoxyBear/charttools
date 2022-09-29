using ChartTools.Extensions.Linq;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Tools.RealTime
{
    public static class RealTimeCalculator
    {
        public static void SyncAnchors(this IEnumerable<Tempo> tempos, uint resolution, bool preOrdered = false)
        {
            List<Tempo> synced = new(), desynced = new();

            foreach (Tempo tempo in tempos.NonNull())
            {
                if (tempo.PositionSynced)
                    synced.Add(tempo);
                else if (tempo.Anchor == TimeSpan.Zero)
                {
                    tempo.SyncPosition(0);
                    synced.Add(tempo);
                }
                else
                    desynced.Add(tempo);
            }

            if (desynced.Count == 0)
                return;
            if (synced.Count == 0)
                throw new Exception("A tempo marker at position or anchor zero is required to sync anchors.");

            using var desyncedEnumerator = desynced.OrderBy(t => t.Anchor!.Value).GetEnumerator();
            using var syncedEnumerator = (preOrdered ? (IEnumerable<Tempo>)synced : synced.OrderBy(t => t.Position)).GetEnumerator();

            syncedEnumerator.MoveNext();
            desyncedEnumerator.MoveNext();

            var ticksPerBeat = resolution / 4d;
            var previous = syncedEnumerator.Current;
            var previousMs = 0ul;

            while (syncedEnumerator.MoveNext())
                if (TryInsertDesynced(syncedEnumerator.Current))
                    if (!desyncedEnumerator.MoveNext())
                        return;

            while (syncedEnumerator.MoveNext())
                TryInsertDesynced(syncedEnumerator.Current);

            bool TryInsertDesynced(Tempo next)
            {
                var msPerBeat = previous.Value * 60 / 3d;
                var deltaMs = msPerBeat * ((next.Position - previous.Position) / ticksPerBeat);

                if (desyncedEnumerator.Current.Anchor!.Value.TotalMilliseconds - previousMs < deltaMs)
                {
                    var desynced = desyncedEnumerator.Current;
                    desynced.SyncPosition((uint)((desynced.Anchor.Value.TotalMilliseconds - previous.Position) / msPerBeat * ticksPerBeat));

                    previous = desynced;
                    return true;
                }

                previous = next;
                return false;
            }
        }
    }
}
