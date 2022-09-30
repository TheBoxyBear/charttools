using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Tools.RealTime
{
    public static class RealTimeCalculator
    {
        public static IEnumerable<Tempo> SyncAnchors(this IEnumerable<Tempo> tempos, uint resolution, bool desyncedPreOrdered = false)
        {
            List<Tempo> synced = new(), desynced = new();
            var zeroCount = 0;

            foreach (Tempo tempo in tempos)
            {
                if (tempo.PositionSynced)
                {
                    if (tempo.Position == 0)
                        synced.Insert(zeroCount++, tempo);
                    else
                        synced.Add(tempo);
                }
                else if (tempo.Anchor == TimeSpan.Zero)
                {
                    tempo.SyncPosition(0);
                    synced.Insert(zeroCount++, tempo);
                }
                else
                    desynced.Add(tempo);
            }

            if (desynced.Count == 0)
                foreach (var tempo in synced)
                    yield return tempo;
            if (synced.Count == 0 || synced[0].Position != 0)
                throw new Exception("A tempo marker at position or anchor zero is required to sync anchors.");

            using var desyncedEnumerator = desynced.OrderBy(t => t.Anchor!.Value).GetEnumerator();
            using var syncedEnumerator = (desyncedPreOrdered ? (IEnumerable<Tempo>)synced : synced.OrderBy(t => t.Position)).GetEnumerator();

            syncedEnumerator.MoveNext();
            desyncedEnumerator.MoveNext();

            var previous = syncedEnumerator.Current;
            var previousMs = 0ul;

            yield return syncedEnumerator.Current;

            while (syncedEnumerator.MoveNext())
            {
                while (TryInsertDesynced(syncedEnumerator.Current))
                {
                    yield return desyncedEnumerator.Current;

                    if (!desyncedEnumerator.MoveNext())
                    {
                        do
                            yield return syncedEnumerator.Current;
                        while (syncedEnumerator.MoveNext());

                        yield break;
                    }
                }

                yield return syncedEnumerator.Current;
            }

            while (desyncedEnumerator.MoveNext())
            {
                SyncAnchor();
                yield return desyncedEnumerator.Current;
            }

            bool TryInsertDesynced(Tempo next)
            {
                var deltaMs = previous.Value * 50 / 3 * ((next.Position - previous.Position) / resolution);

                if (desyncedEnumerator.Current.Anchor!.Value.TotalMilliseconds - previousMs <= deltaMs)
                {
                    SyncAnchor();
                    return true;
                }

                previous = next;
                return false;
            }
            void SyncAnchor()
            {
                var desynced = desyncedEnumerator.Current;
                desynced.SyncPosition((uint)((desynced.Anchor!.Value.TotalMilliseconds - previousMs) * previous.Value * resolution / 240000));

                previous = desynced;
            }
        }
    }
}
