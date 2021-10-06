using ChartTools.IO.Chart;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Lyrics
{
    public class Phrase : Chord<Syllable, VocalsPitch>
    {
        public override uint Position
        {
            get => base.Position;
            set
            {
                if (value > EndPosition)
                    throw new ArgumentException("The position cannot be larger than the end position.", nameof(Position));
                base.Position = value;
            }
        }
        private uint? _endPosition;
        public uint? EndPosition
        {
            get => EndPosition;
            set
            {
                if (value < Position)
                    throw new ArgumentException("The end position cannot be less than the position.", nameof(EndPosition));
                _endPosition = value;
            }
        }

        private const string ModifierUnsupportedMessage = "Vocals do not support a modifier";
        /// <summary>
        /// *Required to inherit from <see cref="Chord{TNote, TLaneEnum}"/> but vocals do not support modifiers*
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        [Obsolete("Leftover from Chord<> inheritance. Vocals do not support modifiers")]
        public override byte ModifierKey
        {
            get => throw new NotSupportedException(ModifierUnsupportedMessage);
            set => throw new NotSupportedException(ModifierUnsupportedMessage);
        }

        public override NoteCollection<Syllable, VocalsPitch> Notes { get; }
        protected override bool OpenExclusivity => false;

        /// <summary>
        /// Gets the raw text of all syllables as a single string with spaces between syllables
        /// </summary>
        public string RawText => string.Concat(Notes.Select(n => n.IsWordEnd ? n.RawText + ' ' : n.RawText));

        public Phrase(uint position) : base(position)
        {
            EndPosition = position;
            Notes = new(OpenExclusivity);
        }

        internal override bool ChartModifierSupported() => true;

        internal override IEnumerable<string> GetChartData(ChartParser.WritingSession session, ICollection<byte> ignored) => throw new NotSupportedException("Vocals must be converted to global events to be saved as chart.");

        public IEnumerable<GlobalEvent> ToGlobalEvents()
        {
            yield return new(Position, GlobalEventType.PhraseStart);

            foreach (var note in Notes)
                yield return new(note.Position, GlobalEventType.Lyric, note.RawText);
        }
    }
}
