using ChartTools.Meta;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace ChartTools.Generator;

/// <exclude />
[Generator]
public class MetadataGenerator : IIncrementalGenerator
{
	private const string
		MetadataType         = "Metadata",
		AttributeNamespace   = $"{nameof(ChartTools)}.{nameof(Meta)}",
		TryGetSignature      = $"public static partial bool TryGetFromAttribute({MetadataType} metadata, in ReadOnlySpan<char> key, out string value)",
		TrySetSignature      = $"private static partial bool TrySetFromAttribute({MetadataType} metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value)",
		TryRemoveSignature   = $"private static partial bool TryRemoveFromAttribute({MetadataType} metadata, in ReadOnlySpan<char> key)",
		TryContainsSignature = $"private static partial bool? TryContainsFromAttribute({MetadataType} metadata, in ReadOnlySpan<char> key)",
		GetAllSignature      = $"private partial IEnumerable<TextEntry> GetAllFromAttributes({MetadataType} metadata)";

	private record class MetadataProperty(string Name, string Type, string ParseType, string Path, params ImmutableArray<MetadataKeyAttribute> Keys);

	private record class MetadataKeyBind(MetadataProperty Property, MetadataKeyAttribute Attribute);

	private record class MetatadaMapperClass(string Name, string Namespace, FileType FileType);

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		if (!Debugger.IsAttached)
		{
			// Uncomment this line to be prompted to debug on generation
			//Debugger.Launch();
		}

		IncrementalValueProvider<ImmutableArray<MetadataProperty>> propProvider =
			context.SyntaxProvider.CreateSyntaxProvider(
				predicate: static (node, _) => node is ClassDeclarationSyntax cds && cds.Identifier.Text == MetadataType,
				transform: static (context, ctx) =>
				{
					return context.SemanticModel.GetDeclaredSymbol(context.Node) is not ITypeSymbol rootSymbol
					? [] : GetProps(rootSymbol, string.Empty);

					IEnumerable<MetadataProperty> GetProps(ITypeSymbol classSymbol, string prefix)
					{
						IEnumerable<IPropertySymbol> props = classSymbol.GetMembers()
							.OfType<IPropertySymbol>()
							.Where(static prop => prop.DeclaredAccessibility == Accessibility.Public && !prop.IsReadOnly && !prop.IsAbstract && !prop.IsStatic);

						foreach (IPropertySymbol prop in props)
						{
							if (ctx.IsCancellationRequested)
								yield break;

							string path = $"{prefix}.{prop.Name}";

							AttributeData[] mappingAttributes = [..prop.GetAttributes().Where(attr => attr.AttributeClass?.Name == "MetadataKeyAttribute")];

							if (mappingAttributes.Length > 0 || prop.Type.SpecialType == SpecialType.System_String || prop.Type.IsValueType)
							{
								MetadataKeyAttribute[] keys = new MetadataKeyAttribute[mappingAttributes.Length];

								for (int i = 0; i < mappingAttributes.Length; i++)
								{
									AttributeData mapping = mappingAttributes[i];

									KeyValuePair<string, TypedConstant> mappable = mapping.NamedArguments
										.FirstOrDefault(static arg => arg.Key == "ValueMappable");

									FileType fileType = (FileType)mapping.ConstructorArguments[0].Value!;
									string key = (string)mapping.ConstructorArguments[1].Value!;

									keys[i] = mappable.Key is null
										? new(fileType, key)
										: new(fileType, key) { ValueMappable = (bool)mappable.Value.Value! };
								}

								yield return new MetadataProperty(
									Name: prop.Name,
									Type: prop.Type.ToDisplayString(),
									ParseType: prop.Type.Name == "Nullable"
									  ? ((INamedTypeSymbol)prop.Type).TypeArguments[0].ToDisplayString()
									  : prop.Type.ToDisplayString(),
									Path: path,
									Keys: keys.ToImmutableArray());
							}
							else
								foreach (MetadataProperty subProp in GetProps(prop.Type, path))
									yield return subProp;
						}
					}
				}).SelectMany(static (props, _) => props).Collect();

		IncrementalValuesProvider<MetadataKeyBind> bindProvider = propProvider.SelectMany(static (props, _) =>
		{
			List<MetadataKeyBind> binds = [];

			foreach (MetadataProperty prop in props)
			{
				binds.EnsureCapacity(binds.Count + prop.Keys.Length);
				binds.AddRange(prop.Keys.Select(key => new MetadataKeyBind(
					Property: prop,
					Attribute: key)));
			}

			return binds;
		});

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
			IncrementalValuesProvider<MetadataKeyBind> keyBindProvider = bindProvider
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
				   .Where(static bind => bind.Attribute.ValueMappable).Collect()),
			   (ctx, tuple) => GenerateMapMethods(in ctx, tuple.Left, tuple.Right));

			context.RegisterImplementationSourceOutput(mapperProvider.Combine(keyBindProvider.Collect()),
				(ctx, tuple) => GenerateNonMapMethods(in ctx, tuple.Left, tuple.Right));
		}

		context.RegisterImplementationSourceOutput(propProvider,
			static (ctx, props) => GenerateMergeMethod(in ctx, props));
	}

	#region Mapping
	private static CodeBuilder OpenPartialMapperClass(MetatadaMapperClass mapper)
		=> new CodeBuilder(
$$"""
// <auto-generated/>

using ChartTools.IO;

namespace {{mapper.Namespace}};

internal sealed partial class {{mapper.Name}}

""")
.StartContext('{');

	private static void GenerateMapMethods(in SourceProductionContext context, MetatadaMapperClass mapper, ImmutableArray<MetadataKeyBind> binds)
	{
		CodeBuilder builder = OpenPartialMapperClass(mapper);

		if (context.CancellationToken.IsCancellationRequested)
			return;

		BuildTryGet(builder, in binds);

		if (context.CancellationToken.IsCancellationRequested)
			return;

		BuildTrySet(builder, in binds);

		if (context.CancellationToken.IsCancellationRequested)
			return;

		context.AddSource($"{mapper.Name}_Map.g.cs", builder.ToString());
	}

	private static void GenerateNonMapMethods(in SourceProductionContext context, MetatadaMapperClass mapper, ImmutableArray<MetadataKeyBind> binds)
	{
		CodeBuilder builder = OpenPartialMapperClass(mapper);

		if (context.CancellationToken.IsCancellationRequested)
			return;

		BuildTryRemove(builder, in binds);

		if (context.CancellationToken.IsCancellationRequested)
			return;

		BuildTryContains(builder, in binds);

		if (context.CancellationToken.IsCancellationRequested)
			return;

		BuildGetAll(builder, in binds);

		if (context.CancellationToken.IsCancellationRequested)
			return;

		context.AddSource($"{mapper.Name}_Misc.g.cs", builder.ToString());
	}

	private static void BuildTryGet(CodeBuilder builder, in ImmutableArray<MetadataKeyBind> binds)
	{
		builder.AppendLines(
$$"""
{{TryGetSignature}}
	=> (value = key switch
	{
""");

		foreach (MetadataKeyBind bind in binds)
		{
			string toStringSuffix = bind.Property.Type is "string" or "System.String"
				? string.Empty : "?.ToString()";

			builder.AppendLine(
$"""
		"{bind.Attribute.Key}" => metadata{bind.Property.Path}{toStringSuffix},
""");
		}

		builder.AppendLines(
"""
		_ => null
	}) is not null;
""");
	}

	private static void BuildTrySet(CodeBuilder builder, in ImmutableArray<MetadataKeyBind> binds)
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
			string setCode = bind.Property.Type is "string" or "System.String"
				? "value.ToString()"
				: $"ValueParser.Parse<{bind.Property.ParseType}>(in value, \"{bind.Property.Name}\")";

			builder.AppendLines(
$"""
		case "{bind.Attribute.Key}":
			metadata{bind.Property.Path} = {setCode};
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

	private static void BuildTryRemove(CodeBuilder builder, in ImmutableArray<MetadataKeyBind> binds)
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
			metadata{bind.Property.Path} = default;
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

	private static void BuildTryContains(CodeBuilder builder, in ImmutableArray<MetadataKeyBind> binds)
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
		"{bind.Attribute.Key}" => metadata{bind.Property.Path} is not null,
""");
		}

		builder.AppendLines(
"""
		_ => null
	};
""");
	}

	private static void BuildGetAll(CodeBuilder builder, in ImmutableArray<MetadataKeyBind> binds)
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
	return [];
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
	#endregion

	private static void GenerateMergeMethod(in SourceProductionContext context, ImmutableArray<MetadataProperty> properties)
	{
		StringBuilder builder = new(
$$"""
// <auto-generated/>
namespace ChartTools.Meta;

public sealed partial class {{MetadataType}}
{
	partial void Merge_Impl(params IEnumerable<{{MetadataType}}> metadata)
	{
		foreach ({{MetadataType}} mergeItem in metadata)
		{
			byte remainingProps = {{properties.Length}};

""");

		foreach (MetadataProperty prop in properties)
		{
			builder.AppendLine($$"""
			if (this{{prop.Path}} == default({{prop.Type}}))
			{
				this{{prop.Path}} = mergeItem{{prop.Path}};
				remainingProps--;
			}

""");
		}

		builder.AppendLine("""
			if (remainingProps == 0)
				return;
		}
	}
}
""");

		context.AddSource("Metadata_Merge.g.cs", builder.ToString());
	}
}
