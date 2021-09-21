using ChartTools.IO;
using ChartTools.IO.Chart;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChartTools.Lyrics
{
    /// <summary>
    /// Line of lyrics shown as karaoke
    /// </summary>
    public class Phrase : TrackObject, IEnumerable<Syllable>
    {
        /// <summary>
        /// Position of the PhraseEnd event
        /// </summary>
        /// <remarks>Null if the phrase has no PhraseEnd event</remarks>
        public uint? EndPosition { get; set; }
        /// <summary>
        /// Position of the first syllable
        /// </summary>
        public uint? SyllablesStart => GetSyllablePositions()?.Min();
        /// <summary>
        /// Position of the last syllable
        /// </summary>
        public uint? SyllablesEnd => GetSyllablePositions()?.Max();
        /// <summary>
        /// The phrase as it is displayed in-game
        /// </summary>
        public string DisplayedText
        {
            get
            {
                Syllable syllable = Syllables[0];
                StringBuilder builder = new(Syllables[0].DisplayedText);
                bool addSpace = syllable.IsWordEnd;

                for (int i = 1; i < Syllables.Count; i++)
                {
                    syllable = Syllables[i];

                    builder.Append(addSpace ? $" {syllable.DisplayedText}" : syllable.DisplayedText);
                    addSpace = syllable.IsWordEnd;
                }

                return builder.ToString();
            }
        }
        /// <summary>
        /// Gets the raw text of all syllables as a single string with spaces between syllables
        /// </summary>
        public string RawText => string.Concat(Syllables.Select(s => s.IsWordEnd ? s.RawText + ' ' : s.RawText));

        /// <summary>
        /// Syllables in the <see cref="Phrase"/>
        /// </summary>
        public UniqueTrackObjectCollection<Syllable> Syllables { get; set; } = new();

        /// <summary>
        /// Creates an instance of <see cref="Phrase"/>
        /// </summary>
        /// <param name="position"></param>
        public Phrase(uint position) : base(position) { }

        /// <inheritdoc cref="ChartParser.ReadLyrics(string)"/>
        public static IEnumerable<Phrase> FromFile(string path, ReadingConfiguration? config) => ExtensionHandler.Read(path, config, (".chart", ChartParser.ReadLyrics));

        /// <summary>
        /// Gets a set of <see cref="GlobalEvent"/> that make up the <see cref="Phrase"/>
        /// </summary>
        /// <returns>Enumerable of <see cref="GlobalEvent"/></returns>
        public IEnumerable<GlobalEvent> ToGlobalEvents()
        {
            yield return new GlobalEvent(Position, GlobalEventType.PhraseStart);

            foreach (Syllable syllable in Syllables)
                yield return new GlobalEvent(syllable.Position, GlobalEventType.Lyric, syllable.RawText is null ? string.Empty : syllable.RawText);

            if (EndPosition is not null)
                yield return new GlobalEvent((uint)EndPosition, GlobalEventType.PhraseEnd);
        }

        public IEnumerator<Syllable> GetEnumerator() => Syllables.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private IEnumerable<uint>? GetSyllablePositions() => Syllables is null || Syllables.Count == 0 ? null : Syllables.Select(s => s.Position);
    }
}
