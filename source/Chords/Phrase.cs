using ChartTools.Events;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Lyrics
{
    public class Phrase : Chord<Syllable, VocalsPitch, VocalChordModifier>, ILongObject
    {
        public override List<Syllable> Notes { get; } = new();
        public override uint Position { get; set; }
        /// <summary>
        /// End of the phrase as defined by <see cref="Length"/>
        /// </summary>
        public uint EndPosition => Position + Length;

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
        public string RawText => BuildText(n => n.RawText);
        public string DisplayedText => BuildText(n => n.DisplayedText);

        internal override bool ChartSupportedMoridier => true;

        protected override VocalChordModifier DefaultModifier => VocalChordModifier.None;

        public Phrase(uint position) : base(position) { }

        public IEnumerable<GlobalEvent> ToGlobalEvents()
        {
            yield return new(Position, EventTypeHelper.Global.PhraseStart);

            foreach (var note in Notes)
                yield return new(Position + note.PositionOffset, EventTypeHelper.Global.Lyric, note.RawText);
        }

        private string BuildText(Func<Syllable, string> textSelector) => string.Concat(Notes.Select(n => n.IsWordEnd ? textSelector(n) + ' ' : textSelector(n)));

        internal override IEnumerable<TrackObjectEntry> GetChartNoteData() => throw new InvalidOperationException("Phrases must be converted to events to be written to chart.");
        internal override IEnumerable<TrackObjectEntry> GetChartModifierData(Chord? previous, WritingSession session) => GetChartNoteData();
    }
}
