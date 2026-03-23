using ChartTools.Meta;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace ChartTools.Generator;

/// <exclude />
[Generator]
public class MetadataMapGenerator : IIncrementalGenerator
{
	private const string
		MetadataType         = "Metadata",
		AttributeNamespace   = $"{nameof(ChartTools)}.{nameof(Meta)}",
		TryGetSignature      = $"public static partial bool TryGetFromAttribute({MetadataType} metadata, in ReadOnlySpan<char> key, out string value)",
		TrySetSignature      = $"private static partial bool TrySetFromAttribute({MetadataType} metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value)",
		TryRemoveSignature   = $"private static partial bool TryRemoveFromAttribute({MetadataType} metadata, in ReadOnlySpan<char> key)",
		TryContainsSignature = $"private static partial bool? TryContainsFromAttribute({MetadataType} metadata, in ReadOnlySpan<char> key)",
		GetAllSignature      = $"private partial IEnumerable<TextEntry> GetAllFromAttributes({MetadataType} metadata)";

	private record class MetatadaMapperClass(string Name, string Namespace, FileType FileType);

	private record class MetadataProperty(string Name, string Type, string ContainingType);

	private record class MetadataKeyBind(MetadataProperty Property, MetadataKeyAttribute Attribute);

	private record class MetadataGroupProperty(string Name, string Type, string ContainingType);

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		if (!Debugger.IsAttached)
		{
			// Uncomment this line to be prompted to debug on generation
			//Debugger.Launch();
		}

		IncrementalValueProvider<ImmutableArray<MetadataGroupProperty>> groups =
			context.SyntaxProvider.ForAttributeWithMetadataName(
				$"{AttributeNamespace}.{nameof(MetadataGroupAttribute)}",
				predicate: static (node, _) => node is PropertyDeclarationSyntax,
				transform: static (context, _) => new MetadataGroupProperty(
				Name: context.TargetSymbol.Name,
				Type: (context.TargetSymbol as IPropertySymbol)!.Type.Name,
				ContainingType: context.TargetSymbol.ContainingType.Name))
			.Collect();

		IncrementalValuesProvider<MetadataKeyBind> provider =
			context.SyntaxProvider.ForAttributeWithMetadataName($"{AttributeNamespace}.{nameof(MetadataKeyAttribute)}",
				predicate: static (node, _) => node is PropertyDeclarationSyntax,
				transform: static (context, _) =>
				{
					IPropertySymbol target = (IPropertySymbol)context.TargetSymbol;

					MetadataProperty property = new(
						Name: target.Name,
						Type: target.Type.TypeKind == TypeKind.Class
						? target.Type.Name
						// Extract the underlying type from Nullable<T>
						: (target.Type as INamedTypeSymbol)!.TypeArguments[0].Name,
						ContainingType: target.ContainingType.Name);

					return context.Attributes.Select(
						attribute =>
						{
							KeyValuePair<string, TypedConstant> mappable =
								attribute.NamedArguments
									.FirstOrDefault(static arg => arg.Key == nameof(MetadataKeyAttribute.ValueMappable));

							FileType attFileType = (FileType)attribute.ConstructorArguments[0].Value!;
							string attKey = (string)attribute.ConstructorArguments[1].Value!;

							return new MetadataKeyBind(property, mappable.Key is null
								? new(attFileType, attKey)
								: new(attFileType, attKey) { ValueMappable = (bool)mappable.Value.Value! });
						}).ToImmutableArray();
				}).SelectMany(static (binds, _) => binds);

		IncrementalValuesProvider<MetatadaMapperClass> mappers =
			context.SyntaxProvider.ForAttributeWithMetadataName(
				$"{AttributeNamespace}.{nameof(MetadataMapperAttribute)}",
				predicate: static (node, _) => node is ClassDeclarationSyntax,
				transform: static (context, _) =>
				{
					ITypeSymbol target = (ITypeSymbol)context.TargetSymbol;
					AttributeData attribute = context.Attributes.First();

					return new MetatadaMapperClass(
						Name: target.Name,
						Namespace: target.ContainingNamespace.ToDisplayString(),
						FileType: (FileType)attribute.ConstructorArguments[0].Value!);
				});

		RegisterMapper(FileType.Chart);
		RegisterMapper(FileType.Ini);

		void RegisterMapper(FileType fileType)
		{
			IncrementalValuesProvider<MetadataKeyBind> keyBindProvider = provider
				.Where(bind => bind.Attribute.FileType == fileType);

			IncrementalValuesProvider<MetatadaMapperClass> mapperProvider = mappers
				.Where(mapper => mapper.FileType == fileType);

			context.RegisterSourceOutput(mapperProvider,
				(ctx, mapper) =>
				{
					CodeBuilder builder = OpenPartialMapperClass(mapper)
						.AppendInstruction(TryGetSignature)
						.AppendInstruction(TrySetSignature)
						.AppendInstruction(TryRemoveSignature)
						.AppendInstruction(TryContainsSignature)
						.AppendInstruction(GetAllSignature);

					if (ctx.CancellationToken.IsCancellationRequested)
						return;

					ctx.AddSource($"{mapper.Name}_Def.cs", builder.ToString());
				});

			context.RegisterImplementationSourceOutput(
				mapperProvider
				.Combine(keyBindProvider
				   .Where(static bind => bind.Attribute.ValueMappable).Collect()
				   .Combine(groups)),
			   (ctx, tuple) => GenerateMapMethods(tuple.Left, in ctx, tuple.Right));

			context.RegisterImplementationSourceOutput(mapperProvider.Combine(keyBindProvider.Collect().Combine(groups)),
				(ctx, tuple) => GenerateNonMapMethods(tuple.Left, in ctx, tuple.Right));
		}
	}

	private static CodeBuilder OpenPartialMapperClass(MetatadaMapperClass mapper)
		=> new CodeBuilder(
$$"""
// <auto-generated/>

using ChartTools.IO;

namespace {{mapper.Namespace}};

internal sealed partial class {{mapper.Name}}

""")
.StartContext('{');

	private static Dictionary<string, string> GetGroupPaths(in ImmutableArray<MetadataGroupProperty> groups)
	{
		Dictionary<string, string> paths = new(groups.Length);

		do
			foreach (MetadataGroupProperty group in groups)
			{
				if (paths.ContainsKey(group.Type))
					continue;

				if (group.ContainingType is MetadataType)
					paths[group.Type] = $".{group.Name}";
				else if (paths.TryGetValue(group.ContainingType, out var path))
					paths[group.Type] = $"{path}.{group.Name}";
			}
		while (paths.Count < groups.Length);

		// Shorthand to not have to check for root
		paths[MetadataType] = string.Empty;

		return paths;
	}

	private static void GenerateMapMethods
		(MetatadaMapperClass mapper, in SourceProductionContext context,
		(ImmutableArray<MetadataKeyBind>, ImmutableArray<MetadataGroupProperty>) tuple)
	{
		var (binds, groups) = tuple;

		Dictionary<string, string> paths = GetGroupPaths(in groups);

		CodeBuilder builder = OpenPartialMapperClass(mapper);

		if (context.CancellationToken.IsCancellationRequested)
			return;

		BuildTryGet(builder, in binds, paths);

		if (context.CancellationToken.IsCancellationRequested)
			return;

		BuildTrySet(builder, in binds, paths);

		if (context.CancellationToken.IsCancellationRequested)
			return;

		context.AddSource($"{mapper.Name}_Map.g.cs", builder.ToString());
	}

	private static void GenerateNonMapMethods
		(MetatadaMapperClass mapper, in SourceProductionContext context,
		(ImmutableArray<MetadataKeyBind>, ImmutableArray<MetadataGroupProperty>) tuple)
	{
		var (binds, groups) = tuple;

		Dictionary<string, string> paths = GetGroupPaths(in groups);

		CodeBuilder builder = OpenPartialMapperClass(mapper);

		if (context.CancellationToken.IsCancellationRequested)
			return;

		BuildTryRemove(builder, in binds, paths);

		if (context.CancellationToken.IsCancellationRequested)
			return;

		BuildTryContains(builder, in binds, paths);

		if (context.CancellationToken.IsCancellationRequested)
			return;

		BuildGetAll(builder, in binds, paths);

		if (context.CancellationToken.IsCancellationRequested)
			return;

		context.AddSource($"{mapper.Name}_Misc.g.cs", builder.ToString());
	}

	private static void BuildTryGet
		(CodeBuilder builder, in ImmutableArray<MetadataKeyBind> binds, Dictionary<string, string> paths)
	{
		builder.AppendLines(
$$"""
{{TryGetSignature}}
	=> (value = key switch
	{
""");

		foreach (MetadataKeyBind bind in binds)
		{
			string toStringSuffix = bind.Property.Type is "String"
				? string.Empty : "?.ToString()";

			builder.AppendLine(
$"""
		"{bind.Attribute.Key}" => metadata{paths![bind.Property.ContainingType]}.{bind.Property.Name}{toStringSuffix},
""");
		}

		builder.AppendLines(
"""
		_ => null
	}) is not null;
""");
	}

	private static void BuildTrySet
		(CodeBuilder builder, in ImmutableArray<MetadataKeyBind> binds, Dictionary<string, string> paths)
	{
		builder.AppendLines(
$$"""
{{TrySetSignature}}
{
	switch (key)
	{
""");

		foreach (MetadataKeyBind bind in binds)
		{
			string setCode = bind.Property.Type == "String"
				? "value.ToString()"
				: $"ValueParser.Parse<{bind.Property.Type}>(in value, \"{bind.Property.Name}\")";

			builder.AppendLines(
$"""
		case "{bind.Attribute.Key}":
			metadata{paths![bind.Property.ContainingType]}.{bind.Property.Name} = {setCode};
			return true;
""");
		}

		builder.AppendLines(
"""
		default:
			return false;
	}
}
""");
	}

	private static void BuildTryRemove
		(CodeBuilder builder, in ImmutableArray<MetadataKeyBind> binds, Dictionary<string, string> paths)
	{
		builder.AppendLines(
$$"""
{{TryRemoveSignature}}
{
	switch (key)
	{
""");

		foreach (MetadataKeyBind bind in binds)
		{
			builder.AppendLines(
$"""
		case "{bind.Attribute.Key}":
			metadata{paths![bind.Property.ContainingType]}.{bind.Property.Name} = null;
			return true;
""");
		}

		builder.AppendLines(
"""
		default:
			return false;
	}
}
""");
	}

	private static void BuildTryContains
		(CodeBuilder builder, in ImmutableArray<MetadataKeyBind> binds, Dictionary<string, string> paths)
	{
		builder.AppendLines(
$$"""
{{TryContainsSignature}}
	=> key switch
	{
""");

		foreach (MetadataKeyBind bind in binds)
		{
			builder.AppendLine(
$"""
		"{bind.Attribute.Key}" => metadata{paths![bind.Property.ContainingType]}.{bind.Property.Name} is not null,
""");
		}

		builder.AppendLines(
"""
		_ => null
	};
""");
	}

	private static void BuildGetAll
		(CodeBuilder builder, in ImmutableArray<MetadataKeyBind> binds, Dictionary<string, string> paths)
	{
		builder.AppendLines(
$$"""
{{GetAllSignature}}
{
	string value;
""");

		if (binds.Length == 0)
		{
			builder.AppendLine(
"""
	return Eumerable.Empty<TextEntry>();
""");

			return;
		}

		foreach (MetadataKeyBind bind in binds)
		{
			builder.AppendLines(
$"""
	if ((value = Get(metadata, "{bind.Attribute.Key}")) is not null)
		yield return new("{bind.Attribute.Key}", value);
""");
		}

		builder.AppendLine(
"""
}
""");
	}
}
