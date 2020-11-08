using ChartTools.Collections;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously
    /// </summary>
    public class NoteCollection<TNote> : UniqueList<TNote> where TNote : Note
    {
        /// <summary>
        /// If <see langword="true"/>, trying to combine an open note with other notes will remove the current ones.
        /// </summary>
        public bool OpenExclusivity { get; }

        /// <summary>
        /// Creates an instance of <see cref="NoteCollection{TNote}"/>.
        /// </summary>
        /// <param name="openExclusivity">Value of <see cref="OpenExclusivity"/></param>
        public NoteCollection(bool openExclusivity) : base((a, b) => a.NoteIndex.CompareTo(b.NoteIndex)) => OpenExclusivity = openExclusivity;

        /// <summary>
        /// Adds a note to the <see cref="NoteCollection{TNote}"/>.
        /// </summary>
        /// <remarks>Adding a note that already exists will overwrite the existing note.
        ///     <para>If <see cref="OpenExclusivity"/> is <see langword="true"/>, combining an open note with other notes will remove the current ones.</para>
        /// </remarks>
        /// <param name="item">Item to add</param>
        public new void Add(TNote item)
        {
            if (OpenExclusivity && (item.NoteIndex == 0 || Count > 0 && this[0].NoteIndex == 0))
                Clear();

            base.Add(item);
        }
    }
}
