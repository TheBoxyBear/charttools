using System.Reflection;

namespace ChartTools.IO;

/// <summary>
/// Indicates that a property should be serialized with a specific key in a specific file format
/// </summary>
/// <param name="key"></param>
public abstract class KeySerializableAttribute(string key) : Attribute
{
    /// <summary>
    /// Target format
    /// </summary>
	public abstract FileType Format { get; }

    /// <summary>
    /// Target key
    /// </summary>
	public string Key { get; } = key;

	/// <summary>
	/// Generates groups of non-null property values and their serialization keys.
	/// </summary>
	/// <param name="source">Object containing the properties</param>
	protected static IEnumerable<(string key, string value)> GetSerializable<TAttribute>(object source)
        where TAttribute : KeySerializableAttribute =>
		from prop in source.GetType().GetProperties()
		let att = prop.GetCustomAttribute<TAttribute>()
		where att is not null
		let value = prop.GetValue(source)
		where value is not null
		select (att.Key, att.GetValueString(value));

	protected abstract string GetValueString(object propValue);
}
