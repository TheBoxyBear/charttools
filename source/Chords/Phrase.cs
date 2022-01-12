using ChartTools.IO.Configuration.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Lyrics
{
    public class Phrase : Chord<Syllable, VocalsPitch, VocalChordModifier>, ILongTrackObject
    {
        public override UniqueTrackObjectCollection<Syllable> Notes { get; } = new();

        public override uint Position
        {
            get => base.Position;
            set
            {
                if (value > EndPosition)
                    throw new ArgumentException("Position cannot be larger than the end position.", nameof(Position));
                base.Position = value;
            }
        }
        public uint EndPosition => EndPositionOverride ?? SyllableEnd;
        public uint? EndPositionOverride
        {
            get => _endPositionOverride;
            set
            {
                if (value is null || value >= Position)
                    _endPositionOverride = value;

                if (value < SyllableEnd)
                    throw new ArgumentException("End position cannot be less than the position of the last syllable.", nameof(value));

                throw new ArgumentException("Pnd position cannot be less than the position.", nameof(value));
            }
        }
        private uint? _endPositionOverride;
        public uint Length
        {
            get => EndPosition - Position;
            set
            {
                var syllableEnd = SyllableEnd;

                if (value < syllableEnd)
                    throw new ArgumentException("Length must be long enough to fit all syllables.", nameof(value));

                if (value > syllableEnd)
                    _endPositionOverride = Position + value;
            }
        }

        public uint SyllableStart => Notes.Count == 0 ? Position : Notes.Select(s => s.Position).Min();
        public uint SyllableEnd => Notes.Count == 0 ?  Position : Notes.Select(s => s.EndPosition).Max();

        /// <summary>
        /// Gets the raw text of all syllables as a single string with spaces between syllables
        /// </summary>
        public string RawText => string.Concat(Notes.Select(n => n.IsWordEnd ? n.RawText + ' ' : n.RawText));

        internal override bool ChartSupportedMoridier => true;

        protected override VocalChordModifier DefaultModifier => VocalChordModifier.None;

        public Phrase(uint position) : base(position) { }

        public IEnumerable<GlobalEvent> ToGlobalEvents()
        {
            yield return new(Position, GlobalEventType.PhraseStart);

            foreach (var note in Notes)
                yield return new(note.Position, GlobalEventType.Lyric, note.RawText);
        }

        internal override IEnumerable<string> GetChartNoteData() => Enumerable.Empty<string>();
        internal override IEnumerable<string> GetChartModifierData(Chord? previous, WritingSession session) => Enumerable.Empty<string>();
    }
}
