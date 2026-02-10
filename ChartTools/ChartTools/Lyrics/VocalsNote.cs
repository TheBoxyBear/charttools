using ChartTools.Extensions.Enums;

namespace ChartTools.Lyrics;

/// <summary>
/// Note of a vocals track defining the pitch and displayed text of a single syllable.
/// </summary>
public class VocalsNote(uint position, VocalsPitch pitch = VocalsPitch.None, string? text = null)
	: INote, ILongTrackObject
{
	public uint Position { get; set; } = position;

	public uint Length { get; set; }

#if !NETCOREAPP3_0_OR_GREATER
	uint ILongTrackObject.EndPosition => Position + Length;
#endif

	public SafeEnum<VocalsPitch> Pitch { get; set; } = pitch;

	public byte Index
	{
		get => (byte)Pitch.Value;
		set => Pitch = (VocalsPitch)value;
	}

	/// <summary>
	/// Raw text data of the syllable
	/// </summary>
	public string RawText { get; set; } = text ?? string.Empty;

	/// <summary>
	/// Syllable text formatted to its in-game appearance
	/// </summary>
	/// <remarks>Some special characters may remain. See <see href="https://github.com/TheNathannator/GuitarGame_ChartFormats/blob/main/doc/FileFormats/.mid/Standard/Vocals.md">Vocals format documentation</see> for more information.</remarks>
	// Duplicates the string up to four times. Can be optimized by editing a char buffer directly and rebuilding a string from it.
	// Low-level equivalents of Replace and Trim may also exist for char collections.
	public string DisplayedText => RawText
		.Replace("-", "")
		.Replace('=', '-')
		.Replace('§', '‿')
		.Trim('+', '#', '^', '*', '%');

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
}
