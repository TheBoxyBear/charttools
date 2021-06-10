using ChartTools.Collections.Unique;
using ChartTools.IO;
using ChartTools.IO.Chart;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Lyrics
{
    /// <summary>
    /// Line of lyrics shown as karaoke
    /// </summary>
    public class Phrase : TrackObject
    {
        /// <summary>
        /// Position of the PhraseEnd event
        /// </summary>
        /// <remarks>Null if the phrase has no PhraseEnd event</remarks>
        public uint? EndPosition { get; set; }
        /// <summary>
        /// Position of the first syllable
        /// </summary>
        public uint? SyllablesStart => Syllables is null || Syllables.Count == 0 ? null : (uint?)Syllables.Select(s => s.Position).Min();
        /// <summary>
        /// Position of the last syllable
        /// </summary>
        public uint? SyllablesEnd => Syllables is null || Syllables.Count == 0 ? null : (uint?)Syllables.Select(s => s.Position).Max();
        /// <summary>
        /// The phrase as it is displayed in-game
        /// </summary>
        public string DisplayedText
        {
            get
            {
                Syllable syllable = Syllables[0];
                string output = Syllables[0].DisplayedText;
                bool addSpace = syllable.IsWordEnd && syllable.RawText[^1] != ',';

                for (int i = 1; i < Syllables.Count; i++)
                {
                    syllable = Syllables[i];

                    output += addSpace ? $" {syllable.DisplayedText}" : syllable.DisplayedText;
                    addSpace = syllable.IsWordEnd && syllable.RawText[^1] != ',';
                }

                return output.Trim();
            }
        }
        /// <summary>
        /// Gets the raw text of all syllables as a single string with spaces between syllables
        /// </summary>
        public string RawText => string.Join("", Syllables.Select(s => s.IsWordEnd ? s.RawText.Trim() + ' ' : s.RawText.Trim())).TrimEnd();

        /// <summary>
        /// Syllables in the <see cref="Phrase"/>
        /// </summary>
        public UniqueList<Syllable> Syllables { get; set; } = new UniqueList<Syllable>((s, other) => s.Equals(other));

        /// <summary>
        /// Creates an instance of <see cref="Phrase"/>
        /// </summary>
        /// <param name="position"></param>
        public Phrase(uint position) : base(position) { }

        /// <inheritdoc cref="ChartParser.ReadLyrics(string)"/>
        public static IEnumerable<Phrase> FromFile(string path) => ExtensionHandler.Read(path, (".chart", ChartParser.ReadLyrics));

        /// <summary>
        /// Gets a set of <see cref="GlobalEvent"/> that make up the <see cref="Phrase"/>
        /// </summary>
        /// <returns>Enumerable of <see cref="GlobalEvent"/></returns>
        public IEnumerable<GlobalEvent> ToGlobalEvents()
        {
            yield return new GlobalEvent(Position, GlobalEventType.PhraseStart);

            foreach (Syllable syllable in Syllables)
                yield return new GlobalEvent(syllable.Position, GlobalEventType.Lyric, syllable.RawText);

            if (EndPosition is not null)
                yield return new GlobalEvent((uint)EndPosition, GlobalEventType.PhraseEnd);
        }
    }
}
