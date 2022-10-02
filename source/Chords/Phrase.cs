using ChartTools.Events;

using Melanchall.DryWetMidi.MusicTheory;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Lyrics
{
    public class Phrase : TrackObjectBase, IChord, ILongTrackObject
    {
        public List<Syllable> Syllables { get; } = new();
        IReadOnlyCollection<INote> IChord.Notes => Syllables;

        /// <summary>
        /// End of the phrase as defined by <see cref="Length"/>
        /// </summary>
        public uint EndPosition => Position + Length;

        public uint Length => LengthOverride ?? SyllableEndOffset;
        uint ILongObject.Length
        {
            get => Length;
            set => LengthOverride = value;
        }

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
        public uint SyllableStartOffset => Syllables.Count == 0 ? 0 : Syllables.Select(s => s.PositionOffset).Min();
        /// <summary>
        /// Offset of the end of the last syllable
        /// </summary>
        public uint SyllableEndOffset => Syllables.Count == 0 ? 0 : Syllables.Select(s => s.EndPositionOffset).Max();
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

        public Phrase() : base() { }
        public Phrase(uint position) : base(position) { }

        INote IChord.CreateNote(byte index, uint length)
        {
            var syllable = new Syllable(SyllableEndOffset, VocalPitchValue.A2) { Length = length };
            Syllables.Add(syllable);
            return syllable;
        }

        public IEnumerable<GlobalEvent> ToGlobalEvents()
        {
            yield return new(Position, EventTypeHelper.Global.PhraseStart);

            foreach (var syllable in Syllables)
                yield return new(Position + syllable.PositionOffset, EventTypeHelper.Global.Lyric, syllable.RawText);
        }

        private string BuildText(Func<Syllable, string> textSelector) => string.Concat(Syllables.Select(n => n.IsWordEnd ? textSelector(n) + ' ' : textSelector(n)));

        INote IChord.CreateNote(byte index, uint length = 0) => new Syllable(0, (VocalPitchValue)index) { Length = length };
    }

    /// <summary>
    /// Provides additional methods to <see cref="Phrase"/>
    /// </summary>
    public static class PhraseExtensions
    {
        /// <summary>
        /// Converts a set of <see cref="Phrase"/> to a set of <see cref="GlobalEvent"/> making up the phrases.
        /// </summary>
        /// <param name="source">Phrases to convert into global events</param>
        /// <returns>Global events making up the phrases</returns>
        public static IEnumerable<GlobalEvent> ToGlobalEvents(this IEnumerable<Phrase> source) => source.SelectMany(p => p.ToGlobalEvents());
    }
}
