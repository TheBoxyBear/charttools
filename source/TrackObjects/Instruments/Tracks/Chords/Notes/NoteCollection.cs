using System;
using System.Linq;

using ChartTools.Collections.Unique;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously
    /// </summary>
    public class NoteCollection<TNote, TLane> : UniqueList<TNote> where TNote : Note where TLane : struct
    {
        /// <summary>
        /// If <see langword="true"/>, trying to combine an open note with other notes will remove the current ones.
        /// </summary>
        public bool OpenExclusivity { get; }

        /// <summary>
        /// Creates an instance of <see cref="NoteCollection{TNote}"/>.
        /// </summary>
        /// <param name="openExclusivity">Value of <see cref="OpenExclusivity"/></param>
        public NoteCollection(bool openExclusivity) : base((a, b) => a.NoteIndex.Equals(b.NoteIndex)) => OpenExclusivity = openExclusivity;

        /// <summary>
        /// Adds a note to the <see cref="NoteCollection{TNote}"/>.
        /// </summary>
        /// <remarks>Adding a note that already exists will overwrite the existing note.
        ///     <para>If <see cref="OpenExclusivity"/> is <see langword="true"/>, combining an open note with other notes will remove the current ones.</para>
        /// </remarks>
        /// <param name="item">Item to add</param>
        public override void Add(TNote item)
        {
            if (item is null)
                throw GetNullNoteException(nameof(item));

            if (OpenExclusivity && (item.NoteIndex == 0 || Count > 0 && this[0].NoteIndex == 0))
                Clear();

            base.Add(item);
        }
        public bool Remove(TLane lane)
        {
            TNote? n = this[lane];
            return n is not null && Remove(n);
        }

        public virtual bool Contains(TLane lane) => this.Any(n => n.NoteIndex.Equals(lane));

        public virtual TNote? this[TLane lane] => this.FirstOrDefault(n => n.NoteIndex == Convert.ToByte(lane));

        public static Exception GetNullNoteException(string paramName) => new ArgumentNullException(paramName, "Note is null.");
    }
}
