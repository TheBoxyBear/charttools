using ChartTools.IO.Serializaiton;
using System.Collections.Generic;

namespace ChartTools.IO.Ini;

public class IniKeySerializableAttribute : KeySerializableAttribute
{
    public override FileType FileType => FileType.Ini;

    public IniKeySerializableAttribute(string key) : base(key) { }

    protected override string GetValueString(object propValue) => propValue.ToString()!;

    public static IEnumerable<(string key, string value)> GetSerializable(object source) => GetSerializable<IniKeySerializableAttribute>(source);
}
