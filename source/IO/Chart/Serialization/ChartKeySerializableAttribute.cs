﻿using ChartTools.IO.Serializaiton;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Serialization;

public class ChartKeySerializableAttribute : KeySerializableAttribute
{
    public override FileType FileType => FileType.Chart;

public ChartKeySerializableAttribute(string key) : base(key) { }

protected override string GetValueString(object propValue)
{
    var propString = propValue.ToString()!;
    return propValue is string ? $"\"{propString}\"" : propString;
}

public static IEnumerable<(string key, string value)> GetSerializable(object source) => GetSerializable<ChartKeySerializableAttribute>(source);
}