namespace ChartTools;

/// <summary>
/// Interface of notes with a numerical identity
/// </summary>
public interface INote : ILongObject
{
	/// <summary>
	/// Numerical value of the note identity
	/// </summary>
	public byte Index { get; set; }
}
