using System;

namespace ChartTools.Generator;

/// <summary>
/// Indicates that a property should be serialized with a specific key in a specific file format
/// </summary>
/// <param name="key"></param>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
internal class MetadataKeyAttribute(FileType fileType, string key) : Attribute
{
	public FileType FileType { get; } = fileType;

	/// <summary>
	/// Target key
	/// </summary>
	public string Key { get; } = key;

	public bool Mappable { get; init; } = true;
}
