using ChartTools.IO.Ini;

namespace ChartTools.Meta;

/// <summary>
/// Creator of the chart
/// </summary>
public class Charter
{
	/// <summary>
	/// Name of the creator
	/// </summary>
	[ChartKeySerializable(ChartFormatting.Charter)]
	public string? Name { get; set; }

	/// <summary>
	/// Location of the image file to use as an icon in the Clone Hero song browser
	/// </summary>
	[IniKeySerializable(IniFormatting.Icon)]
	public string? Icon { get; set; }

	public override string ToString() => Name ?? string.Empty;
}
