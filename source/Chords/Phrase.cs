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
                if (EndPositionOverride is not null && value > EndPositionOverride)
                    throw new ArgumentException("Position cannot be larger than the end position override.", nameof(Position));

                base.Position = value;
            }
        }
        /// <summary>
        /// End of the phrase as defined by <see cref="SyllableEndOffset"/>, unless overridden by <see cref="EndPositionOverride"/>
        /// </summary>
        public uint EndPosition => EndPositionOverride ?? SyllableEndOffset;
        /// <summary>
        /// Explicit end position overriding the natural one
        /// </summary>
        public uint? EndPositionOverride
        {
            get => _endPositionOverride;
            set
            {
                if (value is null || value >= Position)
                    _endPositionOverride = value;

                if (value < SyllableEndOffset)
                    throw new ArgumentException("End position cannot be less than the position of the last syllable.", nameof(value));

                throw new ArgumentException("Pnd position cannot be less than the position.", nameof(value));
            }
        }
        private uint? _endPositionOverride;

        public uint Length => LengthOverride ?? SyllableEndOffset;
        public uint? LengthOverride
        {
            get => _lengthOverride;
            set
            {
                if (value is not null && value < SyllableEndOffset)
                    throw new ArgumentException("Length must be large enough to fit all syllables.", nameof(value));

                _lengthOverride = value;
            }
        }
        private uint? _lengthOverride;


        /// <summary>
        /// Offset of the first syllable
        /// </summary>
        public uint SyllableStartOffset => Notes.Count == 0 ? 0 : Notes.Select(s => s.PositionOffset).Min();
        /// <summary>
        /// Offset of the end of the last syllable
        /// </summary>
        public uint SyllableEndOffset => Notes.Count == 0 ? 0 : Notes.Select(s => s.EndPositionOffset).Max();
        /// <summary>
        /// Start position of the first syllable
        /// </summary>
        public uint SyllableStartPosition => SyllableStartOffset + Position;
        /// <summary>
        /// End position of the last syllable
        /// </summary>
        public uint SyllableEndPosition => SyllableEndOffset + Position;

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
