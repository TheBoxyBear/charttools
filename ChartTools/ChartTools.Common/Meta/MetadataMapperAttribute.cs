namespace ChartTools.Meta;

[AttributeUsage(AttributeTargets.Class)]
internal class MetadataMapperAttribute(FileType fileType) : Attribute
{
	public FileType FileType { get; } = fileType;
}
