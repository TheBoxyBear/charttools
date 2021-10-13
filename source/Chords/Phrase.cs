using ChartTools.IO.Chart;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Lyrics
{
    public class Phrase : Chord<Syllable, VocalsPitch>
    {
        public override UniqueTrackObjectCollection<Syllable> Notes { get; }

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
            get => _endPosition;
            set
            {
                if (value < Position)
                    throw new ArgumentException("The end position cannot be less than the position.", nameof(EndPosition));
                _endPosition = value;
            }
        }

        public uint SyllableStart => Notes.Count == 0 ? Position : Notes.MinBy(s => s.Position)!.Position;
        public uint SyllableEnd => Notes.Count == 0 ? EndPosition ?? Position : Notes.Select(s => s.Position + s.Length).Max();

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

        /// <summary>
        /// Gets the raw text of all syllables as a single string with spaces between syllables
        /// </summary>
        public string RawText => string.Concat(Notes.Select(n => n.IsWordEnd ? n.RawText + ' ' : n.RawText));

        public Phrase(uint position) : base(position)
        {
            EndPosition = position;
            Notes = new();
        }

        internal override IEnumerable<string> GetChartData(Chord previous, ChartParser.WritingSession session, ICollection<byte> ignored) => throw new NotSupportedException("Vocals must be converted to global events to be saved as chart.");

        public IEnumerable<GlobalEvent> ToGlobalEvents()
        {
            yield return new(Position, GlobalEventType.PhraseStart);

            foreach (var note in Notes)
                yield return new(note.Position, GlobalEventType.Lyric, note.RawText);
        }
    }
}
