using System;

namespace ChartTools.Lyrics
{
    /// <summary>
    /// Karaoke step of a <see cref="Phrase"/>
    /// </summary>
    public class Syllable : Note<VocalsPitch>, ILongTrackObject
    {
        public uint Position { get; set; }
        public uint EndPosition => (this as ILongTrackObject).EndPosition;

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

        public Syllable(uint position) => Position = position;
        public Syllable(uint position, VocalsPitches pitch) : this(position, new VocalsPitch(pitch)) { }
        public Syllable(uint position, VocalsPitch pitch) : base(pitch)
        {
            if (!Enum.IsDefined(pitch.Pitch))
                throw CommonExceptions.GetUndefinedException(pitch.Pitch);

            Position = position;
        }

        public int CompareTo(ITrackObject? other) => Position.CompareTo(other?.Position);

        public bool Equals(ITrackObject? other) => (this as ITrackObject).Equals(other);
    }
}
