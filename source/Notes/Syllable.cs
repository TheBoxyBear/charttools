using System;

namespace ChartTools.Lyrics
{
    /// <summary>
    /// Karaoke step of a <see cref="Phrase"/>
    /// </summary>
    public class Syllable : Note<VocalsPitch>, ITrackObject
    {
        public uint Position { get; set; }

        public VocalsPitch Pitch { get; set; }
        internal override byte NoteIndex => (byte)Pitch.Pitch;

        private string _rawText = string.Empty;

        /// <summary>
        /// Argument of the native <see cref="GlobalEvent"/>
        /// </summary>
        /// <remarks>Setting to <see langword="null"/> will set to an empty string.</remarks>
        public string RawText
        {
            get => _rawText;
            set => _rawText = value is null ? string.Empty : value;
        }
        /// <summary>
        /// The syllable as it is displayed in-game
        /// </summary>
        public string DisplayedText => RawText.Replace("-", "").Replace('=', '-').Trim('+', '#', '^', '*');
        /// <summary>
        /// <see langword="true"/> if is the last syllable or the only syllable of its word
        /// </summary>
        public bool IsWordEnd
        {
            // The last character is not - or =
            get => RawText.Length == 0 || RawText[^1] is '§' or '_' or not '-' and not '=';
            set
            {
                if (value)
                {
                    if (!IsWordEnd)
                        RawText = RawText[..^1];
                }
                else if (IsWordEnd)
                    RawText += '-';
            }
        }

        public Syllable() => Pitch = new();
        public Syllable(uint position, VocalsPitches pitch) : this(position, new VocalsPitch(pitch)) { }
        public Syllable(uint position, VocalsPitch pitch) : base(pitch)
        {
            if (!Enum.IsDefined(pitch.Pitch))
                throw CommonExceptions.GetUndefinedException(pitch.Pitch);

            Position = position;
        }

        public int CompareTo(ITrackObject? other) => Position.CompareTo(other?.Position);
    }
}
