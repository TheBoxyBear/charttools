using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Tools.RealTime
{
    public static class RealTimeCalculator
    {
        /// <summary>
        /// Synchronizes anchored tempo markers by calculating the matching tempo position.
        /// </summary>
        /// <param name="tempos"></param>
        /// <param name="resolution"></param>
        /// <param name="desyncedPreOrdered"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static List<Tempo> SyncAnchors(this IEnumerable<Tempo> tempos, uint resolution, bool desyncedPreOrdered = false)
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
                return synced;
            if (synced.Count == 0 || synced[0].Position != 0)
                throw new Exception("A tempo marker at position or anchor zero is required to sync anchors.");

            using var desyncedEnumerator = desynced.OrderBy(t => t.Anchor!.Value).GetEnumerator();
            using var syncedEnumerator = (desyncedPreOrdered ? (IEnumerable<Tempo>)synced : synced.OrderBy(t => t.Position)).GetEnumerator();
            var reSynced = new List<(int, Tempo)>();

            syncedEnumerator.MoveNext();
            desyncedEnumerator.MoveNext();

            var previous = syncedEnumerator.Current;
            var previousMs = 0ul;
            var index = 1;

            while (syncedEnumerator.MoveNext())
            {
                index++;

                while (TryInsertDesynced(syncedEnumerator.Current))
                    if (!desyncedEnumerator.MoveNext())
                    {
                        InsertResynced();
                        return synced;
                    }
            }

            while (desyncedEnumerator.MoveNext())
                SyncAnchor();

            InsertResynced();
            return synced;

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
                reSynced.Add((index, desynced));
                index++;
            }
            void InsertResynced()
            {
                foreach ((var i, var tempo) in reSynced)
                    synced.Insert(i, tempo);
            }
        }
    }
}
