using ChartTools.IO.Chart;
using ChartTools.IO.Ini;
using ChartTools.Meta;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Reflection;

namespace ChartTools.Tests;

[TestClass]
public class MetadataTests
{
	private readonly Metadata invalidDummy = new();

	[TestMethod, TestCategory(nameof(Metadata.Get))]
	public void GetInvalidFormatException()
		=> Assert.ThrowsException<ArgumentException>(() => invalidDummy.Get(FileType.Midi, "Dummy"));

	[TestMethod, TestCategory(nameof(Metadata.Set))]
	public void SetInvalidFormatException()
		=> Assert.ThrowsException<ArgumentException>(() => invalidDummy.Set(FileType.Midi, string.Empty, string.Empty));

	[TestMethod, TestCategory(nameof(Metadata.Remove))]
	public void RemoveInvalidFormatException()
		=> Assert.ThrowsException<ArgumentException>(() => invalidDummy.Remove(FileType.Midi, "Dummy"));

	[TestMethod, TestCategory(nameof(Metadata.Get))]
	public void GetEmptyKeyException()
		=> Assert.ThrowsException<ArgumentException>(() => invalidDummy.Get(FileType.Chart, string.Empty));

	[TestMethod, TestCategory(nameof(Metadata.Set))]
	public void SetEmptyKeyException()
	   => Assert.ThrowsException<ArgumentException>(() => invalidDummy.Set(FileType.Chart, string.Empty, "Dummy"));

	[TestMethod, TestCategory(nameof(Metadata.Remove))]
	public void RemoveEmptyKeyException()
		=> Assert.ThrowsException<ArgumentException>(() => invalidDummy.Remove(FileType.Chart, string.Empty));

	[TestMethod, TestCategory(nameof(Metadata.Get))]
	public void GetChartFromAttribute()
	{
		if (!typeof(Metadata).GetProperty(nameof(Metadata.Title))!.GetCustomAttributes<MetadataKeyAttribute>().Any())
			Assert.Fail($"{nameof(MetadataKeyAttribute)} not found on {nameof(Metadata.Title)} property.");

		const string expected = "Lorem ipsum";
		Metadata metadata = new() { Title = expected };

		Assert.AreEqual(expected, metadata.Get(FileType.Chart, ChartFormatting.Title));
	}

	[TestMethod, TestCategory(nameof(Metadata.Get))]
	public void GetIniFromAttribute()
	{
		if (!typeof(Metadata).GetProperty(nameof(Metadata.Title))!.GetCustomAttributes<MetadataKeyAttribute>().Any())
			Assert.Fail($"{nameof(MetadataKeyAttribute)} not found on {nameof(Metadata.Title)} property.");

		string expected = "Lorem ipsum";
		Metadata metadata = new() { Title = expected };

		Assert.AreEqual(expected, metadata.Get(FileType.Ini, IniFormatting.Title));
	}

	[TestMethod, TestCategory(nameof(Metadata.Set))]
	public void SetChartFromAttribute()
	{
		if (!typeof(Metadata).GetProperty(nameof(Metadata.Title))!.GetCustomAttributes<MetadataKeyAttribute>().Any())
			Assert.Fail($"{nameof(MetadataKeyAttribute)} not found on {nameof(Metadata.Title)} property.");

		const string expected = "Lorem ipsum";

		Metadata metadata = new();
		metadata.Set(FileType.Chart, ChartFormatting.Title, expected);

		Assert.AreEqual(expected, metadata.Title);
	}

	[TestMethod, TestCategory(nameof(Metadata.Set))]
	public void SetIniFromAttribute()
	{
		if (!typeof(Metadata).GetProperty(nameof(Metadata.Title))!.GetCustomAttributes<MetadataKeyAttribute>().Any())
			Assert.Fail($"{nameof(MetadataKeyAttribute)} not found on {nameof(Metadata.Title)} property.");

		const string expected = "Lorem ipsum";

		Metadata metadata = new();
		metadata.Set(FileType.Ini, IniFormatting.Title, expected);

		Assert.AreEqual(expected, metadata.Title);
	}

	[TestMethod, TestCategory(nameof(Metadata.Remove))]
	public void RemoveChartFromAttribute()
	{
		if (!typeof(Metadata).GetProperty(nameof(Metadata.Title))!.GetCustomAttributes<MetadataKeyAttribute>().Any())
			Assert.Fail($"{nameof(MetadataKeyAttribute)} not found on {nameof(Metadata.Title)} property.");

		Metadata metadata = new() { Title = "Lorem ipsum" };
		metadata.Remove(FileType.Chart, ChartFormatting.Title);

		Assert.IsNull(metadata.Title);
	}

	[TestMethod, TestCategory(nameof(Metadata.Remove))]
	public void RemoveIniFromAttribute()
	{
		if (!typeof(Metadata).GetProperty(nameof(Metadata.Title))!.GetCustomAttributes<MetadataKeyAttribute>().Any())
			Assert.Fail($"{nameof(MetadataKeyAttribute)} not found on {nameof(Metadata.Title)} property.");

		Metadata metadata = new() { Title = "Lorem ipsum" };
		metadata.Remove(FileType.Ini, IniFormatting.Title);

		Assert.IsNull(metadata.Title);
	}

	[TestMethod, TestCategory(nameof(Metadata.Get)), TestCategory(nameof(Metadata.Set))]
	public void UnidentifiedChart()
		=> Unidentified(FileType.Chart);

	[TestMethod, TestCategory(nameof(Metadata.Get)), TestCategory(nameof(Metadata.Set))]
	public void UnidentifiedIni()
		=> Unidentified(FileType.Ini);

	private static void Unidentified(FileType fileType)
	{
		const string
			key		 = "LoremIpsum",
			expected = "Lorem ipsum";

		Metadata metadata = new();
		metadata.Set(fileType, key, expected);

		Assert.AreEqual(expected, metadata.Get(fileType, key));
		Assert.IsNull(metadata.Get(fileType == FileType.Chart ? FileType.Ini : FileType.Chart, key));
	}

	[TestMethod, TestCategory(nameof(Metadata.Remove))]
	public void RemoveUnidentifiedChart()
		=> RemoveUnidentified(FileType.Chart);

	[TestMethod, TestCategory(nameof(Metadata.Remove))]
	public void RemoveUnidentifiedIni()
		=> RemoveUnidentified(FileType.Ini);

	private static void RemoveUnidentified(FileType fileType)
	{
		const string key = "LoremIpsum";

		Metadata metadata = new();
		metadata.Set(fileType, key, "Lorem ipsum");
		metadata.Remove(fileType, key);

		Assert.IsNull(metadata.Get(fileType, key));
	}
}
