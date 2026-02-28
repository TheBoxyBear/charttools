namespace ChartTools.IO.Chart.Serialization;

public class ChartKeySerializableAttribute(string key) : KeySerializableAttribute(key)
{
	public override FileType Format => FileType.Chart;

	protected override string GetValueString(object propValue)
	{
		string propString = propValue.ToString()!;
		return propValue is string ? $"\"{propString}\"" : propString;
	}

	public static IEnumerable<(string key, string value)> GetSerializable(object source)
		=> GetSerializable<ChartKeySerializableAttribute>(source);
}
