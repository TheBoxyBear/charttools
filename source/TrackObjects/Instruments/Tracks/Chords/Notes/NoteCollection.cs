using System;
using System.Linq;

using ChartTools.Collections.Unique;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously
    /// </summary>
    public class NoteCollection<TNote, TNoteEnum> : UniqueList<TNote> where TNote : Note<TNoteEnum> where TNoteEnum : struct, Enum
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

        public TNote? this[TNoteEnum note] => Enum.IsDefined(note) ? this.FirstOrDefault(n => n.NoteIndex == Convert.ToByte(note)) : throw GetNullNoteException(nameof(note));

        public static Exception GetNullNoteException(string paramName) => new ArgumentNullException(paramName, "Note is null.");
    }
}
