namespace ChartTools.Attributes.Metadata;

internal class MetadataIniKeyAttribute(string key) : MetadataKeyAttribute(key)
{
	//public override FileType Format => FileType.Ini;

	protected override string GetValueString(object propValue)
		=> propValue.ToString()!;

	//public static IEnumerable<(string key, string value)> GetSerializable(object source)
	//	=> GetSerializable<MetadataIniKeyAttribute>(source);
}
