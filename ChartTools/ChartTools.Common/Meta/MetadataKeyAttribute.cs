namespace ChartTools.Meta;

/// <summary>
/// Indicates that a property should be serialized with a specific key in a specific file format
/// </summary>
/// <param name="key"></param>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
internal class MetadataKeyAttribute(FileType fileType, string key)
	: Attribute, IEquatable<MetadataKeyAttribute>
{
	public FileType FileType { get; } = fileType;

	/// <summary>
	/// Target key
	/// </summary>
	public string Key { get; } = key;

	public bool ValueMappable = true;

	public bool Equals(MetadataKeyAttribute other) =>
		ReferenceEquals(other, this) ||
		other.FileType == FileType &&
		other.Key == Key &&
		other.ValueMappable == ValueMappable;

	public override bool Equals(object obj)
		=> obj is MetadataKeyAttribute other && Equals(other);
}
