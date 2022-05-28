using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously
    /// </summary>
    public class NoteCollection<TNote, TLane> : ICollection<TNote> where TNote : Note<TLane>, new() where TLane : struct
    {
        protected HashSet<TNote> Notes { get; }

        public int Count => Notes.Count;
        public bool IsReadOnly => false;

        public NoteCollection() => Notes = new(Enumerable.Empty<TNote>(), new FuncEqualityComparer<TNote>((a, b) => a!.NoteIndex == b!.NoteIndex));

        public virtual void Add(TLane lane) => Notes.Add(new() { Lane = lane });
        /// <summary>Adds a note to the collection.</summary>
        /// <exception cref="ArgumentNullException"/>
        public virtual void Add(TNote item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            Notes.Add(item);
        }
        public virtual bool Contains(TLane lane) => Notes.Any(n => n.Lane.Equals(lane));
        public bool Contains(byte noteIndex) => Notes.Any(n => n.NoteIndex.Equals(noteIndex));
        /// <summary>
        /// Determines if a note is contained in the collection.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public bool Contains(TNote item) => item is null ? throw new ArgumentNullException(nameof(item)) : Notes.Contains(item);
        public void Clear() => Notes.Clear();
        public void CopyTo(TNote[] array, int arrayIndex) => Notes.CopyTo(array, arrayIndex);
        /// <summary>
        /// Removes a note from the collection.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public bool Remove(TNote item) => item is null ? throw new ArgumentNullException(nameof(item)) : Notes.Remove(item);
        public virtual bool Remove(TLane lane)
        {
            var note = Notes.FirstOrDefault(n => n.Lane.Equals(lane));
            return note is not null && Notes.Remove(note);
        }
        public IEnumerator<TNote> GetEnumerator() => Notes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Gets the note matching a lane.
        /// </summary>
        public virtual TNote? this[TLane lane] => Notes.FirstOrDefault(n => n.Lane.Equals(lane));
    }
}
