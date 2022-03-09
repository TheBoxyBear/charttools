﻿using ChartTools.Events;
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
                    //throw new ArgumentException("Position cannot be larger than the end position.", nameof(Position));
                base.Position = value;
            }
        }
        /// <summary>
        /// End of the phrase as defined by <see cref="SyllableEnd"/>, unless overridden by <see cref="EndPositionOverride"/>
        /// </summary>
        public uint EndPosition => EndPositionOverride ?? SyllableEnd;
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

        /// <summary>
        /// Start of the first syllable
        /// </summary>
        public uint SyllableStart => Notes.Count == 0 ? Position : Notes.Select(s => s.Position).Min();
        /// <summary>
        /// Natural end position based on the end of the last syllable
        /// </summary>
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
            yield return new(Position, EventTypeHelper.Global.PhraseStart);

            foreach (var note in Notes)
                yield return new(note.Position, EventTypeHelper.Global.Lyric, note.RawText);
        }

        internal override IEnumerable<string> GetChartNoteData() => Enumerable.Empty<string>();
        internal override IEnumerable<string> GetChartModifierData(Chord? previous, WritingSession session) => Enumerable.Empty<string>();
    }
}
