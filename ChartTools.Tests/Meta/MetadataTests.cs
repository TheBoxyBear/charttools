using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.Meta;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Reflection;

namespace ChartTools.Tests.Meta;

[TestClass]
public class MetadataTests
{
	private readonly Metadata invalidDummy = new();

	[TestMethod, TestCategory(nameof(Metadata.Get)), TestCategory(nameof(Exception))]
	public void Get_InvalidFormat_Exception()
		=> Assert.ThrowsException<ArgumentException>(() => invalidDummy.Get(FileType.Midi, "Dummy"));

	[TestMethod, TestCategory(nameof(Metadata.Set)), TestCategory(nameof(Exception))]
	public void Set_InvalidFormat_Exception()
		=> Assert.ThrowsException<ArgumentException>(() => invalidDummy.Set(FileType.Midi, string.Empty, string.Empty));

	[TestMethod, TestCategory(nameof(Metadata.Remove)), TestCategory(nameof(Exception))]
	public void Remove_InvalidFormat_Exception()
		=> Assert.ThrowsException<ArgumentException>(() => invalidDummy.Remove(FileType.Midi, "Dummy"));

	[TestMethod, TestCategory(nameof(Metadata.Get))]
	public void Get_EmptyKey_Exception()
		=> Assert.ThrowsException<ArgumentException>(() => invalidDummy.Get(FileType.Chart, string.Empty));

	[TestMethod, TestCategory(nameof(Metadata.Set))]
	public void Set_EmptyKey_Exception()
	   => Assert.ThrowsException<ArgumentException>(() => invalidDummy.Set(FileType.Chart, string.Empty, "Dummy"));

	[TestMethod, TestCategory(nameof(Metadata.Remove))]
	public void Remove_EmptyKey_Exception()
		=> Assert.ThrowsException<ArgumentException>(() => invalidDummy.Remove(FileType.Chart, string.Empty));

	[TestMethod, TestCategory(nameof(Metadata.Get))]
	[DataRow(FileType.Chart)]
	[DataRow(FileType.Ini)]
	public void Get_FromAttribute_Maps(FileType fileType)
	{
		MetadataKeyAttribute? att = typeof(Metadata).GetProperty(nameof(Metadata.Title))?
			.GetCustomAttributes<MetadataKeyAttribute>()
			.FirstOrDefault(att => att.FileType == fileType);

		if (att is null)
			Assert.Fail($"{nameof(MetadataKeyAttribute)} not found on {nameof(Metadata.Title)} property.");

		const string expected = "Lorem ipsum";
		Metadata metadata = new() { Title = expected };

		Assert.AreEqual(expected, metadata.Get(fileType, att.Key));
	}

	[TestMethod, TestCategory(nameof(Metadata.Set))]
	[DataRow(FileType.Chart)]
	[DataRow(FileType.Ini)]
	public void Set_FromAttribute_Maps(FileType fileType)
	{
		MetadataKeyAttribute? att = typeof(Metadata).GetProperty(nameof(Metadata.Title))?
			.GetCustomAttributes<MetadataKeyAttribute>()
			.FirstOrDefault(att => att.FileType == fileType);

		if (att is null)
			Assert.Fail($"{nameof(MetadataKeyAttribute)} not found on {nameof(Metadata.Title)} property.");

		const string expected = "Lorem ipsum";

		Metadata metadata = new();
		metadata.Set(fileType, att.Key, expected);

		Assert.AreEqual(expected, metadata.Title);
	}

	[TestMethod, TestCategory(nameof(Metadata.Remove))]
	[DataRow(FileType.Chart)]
	[DataRow(FileType.Ini)]
	public void Remove_FromAttribute_Maps(FileType fileType)
	{
		MetadataKeyAttribute? att = typeof(Metadata).GetProperty(nameof(Metadata.Title))?
			.GetCustomAttributes<MetadataKeyAttribute>()
			.FirstOrDefault(att => att.FileType == fileType);

		if (att is null)
			Assert.Fail($"{nameof(MetadataKeyAttribute)} not found on {nameof(Metadata.Title)} property.");


		Metadata metadata = new() { Title = "Lorem ipsum" };
		metadata.Remove(fileType, att.Key);

		Assert.IsNull(metadata.Title);
	}

	[TestMethod, TestCategory(nameof(Metadata.Get))]
	public void Get_ChartYear_Formats()
	{
		const ushort expected = 2000;

		Metadata metadata = new() { Year = expected };

		Assert.AreEqual($"\", {expected}\"", metadata.Get(FileType.Chart, ChartFormatting.Year));
	}

	[TestMethod, TestCategory(nameof(Metadata.Set))]
	public void Set_ChartYear_Formats()
	{
		const ushort expected = 2000;

		Metadata metadata = new();
		metadata.Set(FileType.Chart, ChartFormatting.Year, $"\", {expected}\"");

		Assert.AreEqual(expected, metadata.Year);
	}

	[TestMethod, TestCategory(nameof(Metadata.Set)), TestCategory(nameof(Exception))]
	public void Set_ChartYearInvalid_Exception()
		=> Assert.ThrowsException<ParseException>(() =>
		{
			Metadata metadata = new();
			metadata.Set(FileType.Chart, ChartFormatting.Year, "InvalidYear");
		});

	[TestMethod, TestCategory(nameof(Metadata.Get))]
	public void Get_ChartAudioOffet_Formats()
	{
		TimeSpan offset = TimeSpan.FromSeconds(1);
		string expected = offset.TotalSeconds.ToString();

		Metadata metadata = new() { AudioOffset = offset };

		Assert.AreEqual(expected, metadata.Get(FileType.Chart, ChartFormatting.AudioOffset));
	}

	[TestMethod, TestCategory(nameof(Metadata.Set))]
	public void Set_ChartAudioOffet_Formats()
	{
		TimeSpan expected = TimeSpan.FromSeconds(1);
		string offset = expected.TotalSeconds.ToString();

		Metadata metadata = new();
		metadata.Set(FileType.Chart, ChartFormatting.AudioOffset, offset);

		Assert.AreEqual(expected, metadata.AudioOffset);
	}

	[TestMethod, TestCategory(nameof(Metadata.Set)), TestCategory(nameof(Exception))]
	public void Set_ChartAudioOffsetInvalid_Exception()
		=> Assert.ThrowsException<ParseException>(() =>
		{
			Metadata metadata = new();
			metadata.Set(FileType.Chart, ChartFormatting.Year, "InvalidOffset");
		});

	[TestMethod, TestCategory(nameof(Metadata.Get)), TestCategory(nameof(Metadata.Set))]
	[DataRow(FileType.Chart)]
	[DataRow(FileType.Ini)]
	public void GetSet_Unidentified_Maps(FileType fileType)
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
	[DataRow(FileType.Chart)]
	[DataRow(FileType.Ini)]
	public void Remove_Unidentified_Maps(FileType fileType)
	{
		const string key = "LoremIpsum";

		Metadata metadata = new();
		metadata.Set(fileType, key, "Lorem ipsum");
		metadata.Remove(fileType, key);

		Assert.IsNull(metadata.Get(fileType, key));
	}
}
