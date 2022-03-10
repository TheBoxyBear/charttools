using System;

namespace ChartTools.Lyrics
{
    /// <summary>
    /// Karaoke step of a <see cref="Phrase"/>
    /// </summary>
    public class Syllable : Note<VocalsPitch>
    {
        /// <summary>
        /// Position offset from the <see cref="Phrase"/>
        /// </summary>
        public uint PositionOffset { get; set; }
        /// <summary>
        /// Position offset of the end from the <see cref="Phrase"/>
        /// </summary>
        public uint EndPositionOffset => PositionOffset + Length;

        /// <summary>
        /// Pitch to sing
        /// </summary>
        /// <remarks>Although the octave is specified, some games only require the player to match the key.<br/><br/>Chart files do not support pitches.</remarks>
        public VocalsPitch Pitch { get; set; } = new();
        internal override byte NoteIndex => (byte)Pitch.Pitch;

        private string _rawText = string.Empty;

        /// <summary>
        /// Unformatted text data
        /// </summary>
        /// <remarks>Setting to <see langword="null"/> will set to an empty string.</remarks>
        public string RawText
        {
            get => _rawText;
            set => _rawText = value is null ? string.Empty : value;
        }
        /// <summary>
        /// Text formatted to its in-game appearance
        /// </summary>
        public string DisplayedText => RawText.Replace("-", "").Replace('=', '-').Trim('+', '#', '^', '*');
        /// <summary>
        /// <see langword="true"/> if is the last syllable or the only syllable of its word
        /// </summary>
        public bool IsWordEnd
        {
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

        public Syllable(uint offset) => PositionOffset = offset;
        public Syllable(uint offset, VocalsPitch pitch) : base(pitch)
        {
            if (!Enum.IsDefined(pitch.Pitch))
                throw CommonExceptions.GetUndefinedException(pitch.Pitch);

            PositionOffset = offset;
        }
    }
}
