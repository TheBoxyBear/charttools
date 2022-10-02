using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools
{
    public class TempoMap : IList<Tempo>
    {
        private readonly List<Tempo> _items;

        public Tempo this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }
        public int Count => _items.Count;
        bool ICollection<Tempo>.IsReadOnly => false;
        /// <summary>
        /// Indicates if all anchored markers are synchronized.
        /// </summary>
        public bool Synchronized { get; private set; }

        public void Add(Tempo item)
        {
            Desync();
            _items.Add(item);
            item.Map = this;
        }
        public void AddRange(IEnumerable<Tempo> items)
        {
            Desync();

            foreach (var item in items)
            {
                _items.Add(item);
                item.Map = this;
            }
        }
        public void Clear() => _items.Clear();
        public void Clear(bool detachMap)
        {
            if (detachMap)
                foreach (var tempo in _items)
                    tempo.Map = null;

            _items.Clear();
        }
        public bool Contains(Tempo item) => _items.Contains(item);
        public void CopyTo(Tempo[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
        public int IndexOf(Tempo item) => _items.IndexOf(item);
        public void Insert(int index, Tempo item)
        {
            Desync();
            _items.Insert(index, item);
            item.Map = this;
        }
        public void InsertRange(int index, IEnumerable<Tempo> items)
        {
            Desync();

            foreach (var item in items)
            {
                _items.Insert(index, item);
                item.Map = this;
            }
        }
        public bool Remove(Tempo item) => Remove(item, false);
        public bool Remove(Tempo item, bool detachMap)
        {
            if (detachMap)
                item.Map = null;

            var found = _items.Remove(item);
            Desync();
            return found;
        }
        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
            Desync();
        }
        public void RemoveAt(int index, bool detachMap)
        {
            if (detachMap)
            {
                var tempo = _items[index];
                tempo.Map = null;
            }

            _items.RemoveAt(index);
            Desync();
        }

        public IEnumerator<Tempo> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Synchronizes anchored markers by calculating their tick position.
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="desyncedPreOrdered"></param>
        /// <exception cref="Exception"></exception>
        public void Synchronize(uint resolution, bool desyncedPreOrdered = false)
        {
            if (Synchronized)
                return;

            List<Tempo> synced = new(), desynced = new();
            var zeroCount = 0;

            // Split synced and desynced. Sync 0 anchors.
            foreach (Tempo tempo in _items)
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
                return;

            using var syncedEnumerator = (desyncedPreOrdered ? (IEnumerable<Tempo>)synced : synced.OrderBy(t => t.Position)).GetEnumerator();

            if (!syncedEnumerator.MoveNext() || syncedEnumerator.Current.Position != 0)
                throw new Exception("A tempo marker at position or anchor zero is required to sync anchors.");

            using var desyncedEnumerator = desynced.OrderBy(t => t.Anchor!.Value).GetEnumerator();

            syncedEnumerator.MoveNext();
            desyncedEnumerator.MoveNext();

            var previous = syncedEnumerator.Current;
            var previousMs = 0ul;

            while (syncedEnumerator.MoveNext())
                while (TryInsertDesynced(syncedEnumerator.Current))
                    if (!desyncedEnumerator.MoveNext())
                        return;

            while (desyncedEnumerator.MoveNext())
                SyncAnchor();
            return;

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
        internal void Desync()
        {
            foreach (var tempo in _items)
                tempo.DesyncPosition();

            Synchronized = false;
        }
    }
}
