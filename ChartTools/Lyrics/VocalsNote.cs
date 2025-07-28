namespace ChartTools.Lyrics;

public struct VocalsNote(VocalsPitch pitch, string? text = null)
	: INote, ILongTrackObject
{
	public VocalsNote(string? text = null) : this(VocalsPitchValue.None, text) { }

	public uint Position { readonly get; set; }

	public uint Length { readonly get; set; }

	public VocalsPitch Pitch { readonly get; set; } = pitch;

	readonly byte INote.Index => (byte)Pitch.Value;

	public string RawText { readonly get; set; } = text ?? string.Empty;

	/// <summary>
	/// Text formatted to its in-game appearance
	/// </summary>
	/// <remarks>Some special characters may remain. See <see href="https://github.com/TheNathannator/GuitarGame_ChartFormats/blob/main/doc/FileFormats/.mid/Standard/Vocals.md">Vocals format documentation</see> for more information.</remarks>
	// Duplicates the string up to four times. Can be optimized by editing a char buffer directly and rebuilding a string from it.
	// Low-level equivalents of Replace and Trim may also exist for char collections.
	public readonly string DisplayedText => RawText
		.Replace("-", "")
		.Replace('=', '-')
		.Replace('§', '‿')
		.Trim('+', '#', '^', '*', '%');

	/// <summary>
	/// <see langword="true"/> if is the last syllable or the only syllable of its word
	/// </summary>
	public bool IsWordEnd
	{
		readonly get => RawText.Length == 0 || RawText[^1] is '§' or '_' or not '-' and not '=';
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
}
