using System;
using System.Linq;

namespace ChartTools
{
    public class LaneNoteCollection<TNote, TLane> : NoteCollection<TNote, TLane> where TNote : Note<TLane>, new() where TLane : struct, Enum
    {
        /// <summary>
        /// If <see langword="true"/>, trying to combine an open note with other notes will remove the current ones.
        /// </summary>
        public bool OpenExclusivity { get; }

        public LaneNoteCollection(bool openExclusivity) => OpenExclusivity = openExclusivity;

        public override void Add(TLane lane) => AddNonNull(new TNote() { Lane = lane });

        /// <summary>
        /// Adds a note to the <see cref="NoteCollection{TNote, TLane}"/>.
        /// </summary>
        /// <remarks>Adding a note that already exists will overwrite the existing note.
        ///     <para>If <see cref="OpenExclusivity"/> is <see langword="true"/>, combining an open note with other notes will remove the current ones.</para>
        /// </remarks>
        /// <param name="item">Item to add</param>
        public override void Add(TNote item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            AddNonNull(item);
        }

        private void AddNonNull(TNote item)
        {
            if (OpenExclusivity && (item.NoteIndex == 0 || Count == 1 && this.First().NoteIndex == 0)) // An open note is present and needs to be removed
                Clear();

            base.Add(item);
        }
    }
}
