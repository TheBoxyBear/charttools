using ChartTools.IO.Serializing;

namespace ChartTools.IO.Ini;

public class IniKeySerializableAttribute(string key) : KeySerializableAttribute(key)
{
	public override FileType Format => FileType.Ini;

	protected override string GetValueString(object propValue) => propValue.ToString()!;

	public static IEnumerable<(string key, string value)> GetSerializable(object source)
		=> GetSerializable<IniKeySerializableAttribute>(source);
}
