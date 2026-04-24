using System.Collections.Generic;
using System.Collections.Immutable;

namespace ChartTools.Generator.Metadata;

internal static class MetadataHelper
{
	public const string
		MetadataType = "Metadata",
		AttributeNamespace = $"{nameof(ChartTools)}.{nameof(Meta)}";

	public record class MetadataProperty(string Name, string Type, string ContainingType);

	public record class MetadataGroupProperty(string Name, string Type, string ContainingType);

	public static Dictionary<string, string> GetGroupPaths(in ImmutableArray<MetadataGroupProperty> groups)
	{
		Dictionary<string, string> paths = new(groups.Length);

		do
			foreach (MetadataGroupProperty group in groups)
			{
				if (paths.ContainsKey(group.Type))
					continue;

				if (group.ContainingType is MetadataType)
					paths[group.Type] = $".{group.Name}";
				else if (paths.TryGetValue(group.ContainingType, out string path))
					paths[group.Type] = $"{path}.{group.Name}";
			}
		while (paths.Count < groups.Length);

		// Shorthand to not have to check for root
		paths[MetadataType] = string.Empty;

		return paths;
	}
}
