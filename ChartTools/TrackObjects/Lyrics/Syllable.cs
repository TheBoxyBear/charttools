namespace ChartTools.Lyrics
{
    /// <summary>
    /// Karaoke step of a <see cref="Phrase"/>
    /// </summary>
    public class Syllable : TrackObject
    {
        /// <summary>
        /// Argument of the native <see cref="GlobalEvent"/>
        /// </summary>
        public string RawText { get; set; }
        /// <summary>
        /// The syllable as it is displayed in-game
        /// </summary>
        public string DisplayedText => RawText.Replace("-", "").Replace('=', '-');
        /// <summary>
        /// True if is the last syllable or the only syllable of its word
        /// </summary>
        /// <remarks>Syllable postion is based on the syntax of <see cref="RawText"/></remarks>
        public bool IsWordEnd
        {
            // The last character is not - or =
            get => RawText[^1] is not '-' and not '=';
            set
            {
                if (value)
                {
                    if (IsWordEnd)
                        RawText = RawText[..^1];
                }
                else if (!IsWordEnd)
                    RawText += '-';
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="Syllable"/>.
        /// </summary>
        public Syllable(uint position) : base(position) { }
    }
}
