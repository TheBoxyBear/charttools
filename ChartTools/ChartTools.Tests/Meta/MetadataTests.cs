using ChartTools.Extensions.Enums;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Formatting;
using ChartTools.IO.Ini;
using ChartTools.Meta;

using System.Reflection;

namespace ChartTools.Tests.Meta;

[TestClass]
public class MetadataTests
{
	private static readonly Metadata s_invalidDummy = new();

	#region Invalid format
	[TestMethod, TestCategory(nameof(Metadata)), TestCategory(nameof(Exception))]
	public void TryGet_InvalidFormat_Throws()
		=> Assert.Throws<ArgumentException>(
			static () => s_invalidDummy.TryGet(FileType.Midi, "Dummy", out _));

	[TestMethod, TestCategory(nameof(Metadata.Get)), TestCategory(nameof(Exception))]
	public void Get_InvalidFormat_Throws()
		=> Assert.Throws<ArgumentException>(
			static () => s_invalidDummy.Get(FileType.Midi, "Dummy"));

	[TestMethod, TestCategory(nameof(Metadata.Set)), TestCategory(nameof(Exception))]
	public void Set_InvalidFormat_Throws()
		=> Assert.Throws<ArgumentException>(
			static () => s_invalidDummy.Set(FileType.Midi, string.Empty, string.Empty));

	[TestMethod, TestCategory(nameof(Metadata.Remove)), TestCategory(nameof(Exception))]
	public void Remove_InvalidFormat_Throws()
		=> Assert.Throws<ArgumentException>(
			static () => s_invalidDummy.Remove(FileType.Midi, "Dummy"));

	[TestMethod, TestCategory(nameof(Metadata.Remove)), TestCategory(nameof(Exception))]
	public void Contains_InvalidFormat_Throws()
		=> Assert.Throws<ArgumentException>(
			static () => s_invalidDummy.Contains(FileType.Midi, "Dummy"));
	#endregion

	#region Empty key
	[TestMethod, TestCategory(nameof(Metadata.Get))]
	[TestCategory(nameof(FileType.Chart)), TestCategory(nameof(FileType.Ini))]
	[DataRow(FileType.Chart), DataRow(FileType.Ini)]
	public void Get_EmptyKey_Throws(FileType fileType)
		=> Assert.Throws<ArgumentException>(
			() => s_invalidDummy.Get(fileType, string.Empty));

	[TestMethod, TestCategory(nameof(Metadata.Set)),
		TestCategory(nameof(FileType.Chart)), TestCategory(nameof(FileType.Ini))]
	[DataRow(FileType.Chart), DataRow(FileType.Ini)]
	public void Set_EmptyKey_Throws(FileType fileType)
	   => Assert.Throws<ArgumentException>(
		   () => s_invalidDummy.Set(fileType, string.Empty, "Dummy"));

	[TestMethod, TestCategory(nameof(Metadata.Remove))]
	[TestCategory(nameof(FileType.Chart)), TestCategory(nameof(FileType.Ini))]
	[DataRow(FileType.Chart), DataRow(FileType.Ini)]
	public void Remove_EmptyKey_Throws(FileType fileType)
		=> Assert.Throws<ArgumentException>(
			() => s_invalidDummy.Remove(fileType, string.Empty));

	[TestMethod, TestCategory(nameof(Metadata.Contains))]
	[TestCategory(nameof(FileType.Chart)), TestCategory(nameof(FileType.Ini))]
	[DataRow(FileType.Chart), DataRow(FileType.Ini)]
	public void Contains_EmptyKey_Throws(FileType fileType)
		=> Assert.Throws<ArgumentException>(
			() => s_invalidDummy.Contains(fileType, string.Empty));
	#endregion

	#region FromAttribute
	[TestMethod, TestCategory(nameof(Metadata.Get))]
	[TestCategory(nameof(FileType.Chart)), TestCategory(nameof(FileType.Ini))]
	[DataRow(FileType.Chart), DataRow(FileType.Ini)]
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
	[TestCategory(nameof(FileType.Chart)), TestCategory(nameof(FileType.Ini))]
	[DataRow(FileType.Chart), DataRow(FileType.Ini)]
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
	[TestCategory(nameof(FileType.Chart)), TestCategory(nameof(FileType.Ini))]
	[DataRow(FileType.Chart), DataRow(FileType.Ini)]
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
	#endregion

	#region Audio offset
	[TestMethod, TestCategory(nameof(Metadata.Get)), TestCategory(nameof(Metadata.AudioOffset))]
	[TestCategory(nameof(FileType.Chart)), TestCategory(nameof(FileType.Ini))]
	[DataRow(FileType.Chart, ChartFormatting.AudioOffset, "1")]
	[DataRow(FileType.Ini,	 IniFormatting.AudioOffset, "1000")]
	public void Get_AudioOffet_Formats(FileType fileType, string key, string expected)
	{
		TimeSpan offset = TimeSpan.FromSeconds(1);

		Metadata metadata = new() { AudioOffset = offset };

		Assert.AreEqual(expected, metadata.Get(fileType, key));
	}

	[TestMethod, TestCategory(nameof(Metadata.Set)), TestCategory(nameof(Metadata.AudioOffset))]
	[TestCategory(nameof(FileType.Chart)), TestCategory(nameof(FileType.Ini))]
	[DataRow(FileType.Chart, ChartFormatting.AudioOffset, "1")]
	[DataRow(FileType.Ini,	 IniFormatting.AudioOffset, "1000")]
	public void Set_AudioOffet_Formats(FileType fileType, string key, string value)
	{
		TimeSpan expected = TimeSpan.FromSeconds(1);

		Metadata metadata = new();
		metadata.Set(fileType, key, value);

		Assert.AreEqual(expected, metadata.AudioOffset);
	}

	[TestMethod, TestCategory(nameof(Exception))]
	[TestCategory(nameof(Metadata.Set)), TestCategory(nameof(Metadata.AudioOffset))]
	[TestCategory(nameof(FileType.Chart)), TestCategory(nameof(FileType.Ini))]
	[DataRow(FileType.Chart, ChartFormatting.AudioOffset)]
	[DataRow(FileType.Ini,	 IniFormatting.AudioOffset)]
	public void Set_AudioOffsetInvalid_Throws(FileType fileType, string key)
		=> Assert.Throws<ParseException>(
			() => new Metadata().Set(fileType, key, "InvalidOffset"));
	#endregion

	#region Chart year
	[TestMethod, TestCategory(nameof(FileType.Chart))]
	[TestCategory(nameof(Metadata.Get)), TestCategory(nameof(Metadata.Year))]
	public void Get_ChartYear_Formats()
	{
		const ushort year = 2000;
		string expected   = $"\", {year}\"";

		Metadata metadata = new() { Year = year };

		Assert.AreEqual(expected, metadata.Get(FileType.Chart, ChartFormatting.Year));
	}

	[TestMethod, TestCategory(nameof(FileType.Chart))]
	[TestCategory(nameof(Metadata.Set)), TestCategory(nameof(Metadata.Year))]
	public void Set_ChartYear_Formats()
	{
		const ushort expected = 2000;
		string formatted      = $"\", {expected}\"";

		Metadata metadata = new();
		metadata.Set(FileType.Chart, ChartFormatting.Year, formatted);

		Assert.AreEqual(expected, metadata.Year);
	}

	[TestMethod, TestCategory(nameof(Exception)), TestCategory(nameof(FileType.Chart))]
	[TestCategory(nameof(Metadata.Set)), TestCategory(nameof(Metadata.Year))]
	public void Set_ChartYearInvalid_Throws()
		=> Assert.Throws<ParseException>(
			static () => new Metadata().Set(FileType.Chart, ChartFormatting.Year, "InvalidYear"));
	#endregion

	#region Ini
	#region Video offset
	[TestMethod, TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Get)), TestCategory(nameof(Metadata.VideoOffset))]
	[DataRow(0), DataRow(1), DataRow(-1), DataRow(1000), DataRow(-1000)]
	public void Get_IniVideoOffset_Formats(double milliseconds)
	{
		TimeSpan offset = TimeSpan.FromMilliseconds(milliseconds);
		string expected = offset.TotalMilliseconds.ToString();

		Metadata metadata = new() { VideoOffset = offset };

		Assert.AreEqual(expected, metadata.Get(FileType.Ini, IniFormatting.VideoOffset));
	}

	[TestMethod, TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Set)), TestCategory(nameof(Metadata.VideoOffset))]
	[DataRow(0), DataRow(1), DataRow(-1), DataRow(1000), DataRow(-1000)]
	public void Set_IniVideoOffset_Formats(double milliseconds)
	{
		TimeSpan expected = TimeSpan.FromMilliseconds(milliseconds);
		string value      = expected.TotalMilliseconds.ToString();

		Metadata metadata = new();
		metadata.Set(FileType.Ini, IniFormatting.VideoOffset, value);

		Assert.AreEqual(expected, metadata.VideoOffset);
	}

	[TestMethod, TestCategory(nameof(Exception)), TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Set)), TestCategory(nameof(Metadata.VideoOffset))]
	public void Set_IniVideoOffsetInvalid_Throws()
		=> Assert.Throws<ParseException>(
			static () => new Metadata().Set(FileType.Ini, IniFormatting.VideoOffset, "InvalidOffet"));
	#endregion

	#region Modchart
	[TestMethod, TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Get)), TestCategory(nameof(Metadata.IsModchart))]
	[DataRow(false, "0"), DataRow(true, "1")]
	public void Get_IniModChart_Formats(bool value, string expected)
		=> Assert.AreEqual(expected, new Metadata { IsModchart = value }
			.Get(FileType.Ini, IniFormatting.Modchart));

	[TestMethod, TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Set)), TestCategory(nameof(Metadata.IsModchart))]
	[DataRow(false, "0"), DataRow(true, "1")]
	public void Set_IniModChart_Formats(bool expected, string value)
	{
		Metadata metadata = new();
		metadata.Set(FileType.Ini, IniFormatting.Modchart, value);

		Assert.AreEqual(expected, metadata.IsModchart);
	}

	[TestMethod, TestCategory(nameof(Exception)), TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Set)), TestCategory(nameof(Metadata.IsModchart))]
	public void Set_IniModChartInvalid_Throws()
		=> Assert.Throws<ParseException>(
			static () => new Metadata().Set(FileType.Ini, IniFormatting.Modchart, "InvalidBool"));
	#endregion

	#region Album track
	[TestMethod, TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Get)), TestCategory(nameof(Metadata.AlbumTrack))]
	[DataRow(IniFormatting.Track), DataRow(IniFormatting.AlbumTrack)]
	public void Get_IniTrackNoKeys_ReturnsNull(string key)
	{
		const byte track = 0;

		Metadata metadata = new()
		{
			AlbumTrack = track,
			Formatting = new() { AlbumTrackKeys = AlbumTrackKeys.None }
		};

		Assert.IsNull(metadata.Get(FileType.Ini, key));
	}

	[TestMethod, TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Get)), TestCategory(nameof(Metadata.AlbumTrack))]
	[DataRow(AlbumTrackKeys.Track,		IniFormatting.Track)]
	[DataRow(AlbumTrackKeys.AlbumTrack, IniFormatting.AlbumTrack)]
	[DataRow(AlbumTrackKeys.All,		IniFormatting.Track)]
	[DataRow(AlbumTrackKeys.All,		IniFormatting.AlbumTrack)]
	public void Get_IniTrack_Maps(AlbumTrackKeys keys, string keyString)
	{
		const byte track = 0;
		string expected  = track.ToString();

		Metadata metadata = new()
		{
			AlbumTrack = track,
			Formatting = new() { AlbumTrackKeys = keys }
		};

		Assert.AreEqual(expected, metadata.Get(FileType.Ini, keyString));
	}

	[TestMethod, TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Set)), TestCategory(nameof(Metadata.AlbumTrack))]
	[DataRow(AlbumTrackKeys.Track,		IniFormatting.Track)]
	[DataRow(AlbumTrackKeys.AlbumTrack,	IniFormatting.AlbumTrack)]
	[DataRow(AlbumTrackKeys.All,		IniFormatting.Track)]
	[DataRow(AlbumTrackKeys.All,		IniFormatting.AlbumTrack)]
	public void Set_IniTrack_Maps(AlbumTrackKeys keys, string keyString)
	{
		const byte expected = 0;
		string value = expected.ToString();

		Metadata metadata = new()
		{
			Formatting = new() { AlbumTrackKeys = keys }
		};

		metadata.Set(FileType.Ini, keyString, value);

		Assert.AreEqual(expected, metadata.AlbumTrack);
	}

	[TestMethod, TestCategory(nameof(Exception)), TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Set)), TestCategory(nameof(Metadata.AlbumTrack))]
	public void Set_IniTrackInvalid_Throws()
		=> Assert.Throws<ParseException>(
			() => new Metadata { Formatting = new() { AlbumTrackKeys = AlbumTrackKeys.Track } }
			.Set(FileType.Ini, IniFormatting.Track, "InvalidTrack"));

	[TestMethod, TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Remove)), TestCategory(nameof(Metadata.AlbumTrack))]
	[DataRow(AlbumTrackKeys.Track,		AlbumTrackKeys.Track,	   IniFormatting.Track)]
	[DataRow(AlbumTrackKeys.AlbumTrack, AlbumTrackKeys.AlbumTrack, IniFormatting.AlbumTrack)]
	[DataRow(AlbumTrackKeys.All,		AlbumTrackKeys.Track,	   IniFormatting.Track)]
	[DataRow(AlbumTrackKeys.All,		AlbumTrackKeys.AlbumTrack, IniFormatting.AlbumTrack)]
	public void Remove_IniTrack_RemovesFlag(AlbumTrackKeys original, AlbumTrackKeys removed, string keyString)
	{
		AlbumTrackKeys expected = original.RemoveFlags(removed);

		Metadata metadata = new()
		{
			AlbumTrack = 0,
			Formatting = new() { AlbumTrackKeys = original }
		};

		metadata.Remove(FileType.Ini, keyString);

		Assert.AreEqual(expected, metadata.Formatting.AlbumTrackKeys.Value);
	}

	[TestMethod, TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Remove)), TestCategory(nameof(Metadata.AlbumTrack))]
	[DataRow(AlbumTrackKeys.None,		IniFormatting.Track,	  true)]
	[DataRow(AlbumTrackKeys.None,		IniFormatting.AlbumTrack, true)]
	[DataRow(AlbumTrackKeys.Track,		IniFormatting.Track,	  true)]
	[DataRow(AlbumTrackKeys.AlbumTrack, IniFormatting.AlbumTrack, true)]
	[DataRow(AlbumTrackKeys.All,		IniFormatting.Track,	  false)]
	[DataRow(AlbumTrackKeys.All,		IniFormatting.AlbumTrack, false)]
	public void Remove_IniTrack_RemovesValue(AlbumTrackKeys keys, string keyString, bool expectedRemoved)
	{
		Metadata metadata = new()
		{
			AlbumTrack = 0,
			Formatting = new() { AlbumTrackKeys = keys }
		};

		metadata.Remove(FileType.Ini, keyString);

		Assert.AreEqual(expectedRemoved, metadata.AlbumTrack is null);
	}

	[TestMethod, TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Contains)), TestCategory(nameof(Metadata.AlbumTrack))]
	[DataRow(AlbumTrackKeys.Track,		IniFormatting.Track,	  true)]
	[DataRow(AlbumTrackKeys.AlbumTrack, IniFormatting.AlbumTrack, true)]
	[DataRow(AlbumTrackKeys.All,		IniFormatting.Track,	  true)]
	[DataRow(AlbumTrackKeys.All,		IniFormatting.AlbumTrack, true)]
	[DataRow(AlbumTrackKeys.Track,		IniFormatting.AlbumTrack, false)]
	[DataRow(AlbumTrackKeys.AlbumTrack, IniFormatting.Track,	  false)]
	[DataRow(AlbumTrackKeys.None,		IniFormatting.Track,	  false)]
	[DataRow(AlbumTrackKeys.None,		IniFormatting.AlbumTrack, false)]
	public void Contains_IniTrackMatch_Maps(AlbumTrackKeys keys, string keyString, bool expected)
	{
		Metadata metadata = new()
		{
			AlbumTrack = 0,
			Formatting = new() { AlbumTrackKeys = keys }
		};

		Assert.AreEqual(expected, metadata.Contains(FileType.Ini, keyString));
	}
	#endregion

	#region Charter
	[TestMethod, TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Get)), TestCategory(nameof(Metadata.Charter))]
	[DataRow(IniFormatting.Track), DataRow(IniFormatting.AlbumTrack)]
	public void Get_IniCharterNoKeys_ReturnsNull(string key)
	{
		const string expected = "TheBoxyBear";

		Metadata metadata = new()
		{
			Charter	   = new() { Name = expected },
			Formatting = new() { CharterKeys = CharterKeys.None }
		};

		Assert.IsNull(metadata.Get(FileType.Ini, key));
	}

	[TestMethod, TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Get)), TestCategory(nameof(Metadata.Charter))]
	[DataRow(CharterKeys.Charter, IniFormatting.Charter)]
	[DataRow(CharterKeys.Frets,   IniFormatting.Frets)]
	[DataRow(CharterKeys.All,	  IniFormatting.Charter)]
	[DataRow(CharterKeys.All,	  IniFormatting.Frets)]
	public void Get_IniCharter_Maps(CharterKeys keys, string keyString)
	{
		const string expected = "TheBoxyBear";

		Metadata metadata = new()
		{
			Charter    = new() { Name = expected },
			Formatting = new() { CharterKeys = keys }
		};

		Assert.AreEqual(expected, metadata.Get(FileType.Ini, keyString));
	}

	[TestMethod, TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Set)), TestCategory(nameof(Metadata.Charter))]
	[DataRow(CharterKeys.Charter, IniFormatting.Charter)]
	[DataRow(CharterKeys.Frets,	  IniFormatting.Frets)]
	[DataRow(CharterKeys.All,	  IniFormatting.Charter)]
	[DataRow(CharterKeys.All,	  IniFormatting.Frets)]
	public void Set_IniCharter_Maps(CharterKeys keys, string keyString)
	{
		const string expected = "TheBoxyBear";

		Metadata metadata = new()
		{
			Charter    = new() { Name = expected },
			Formatting = new() { CharterKeys = keys }
		};

		metadata.Set(FileType.Ini, keyString, expected);

		Assert.AreEqual(expected, metadata.Charter.Name);
	}

	[TestMethod, TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Remove)), TestCategory(nameof(Metadata.Charter))]
	[DataRow(CharterKeys.Charter, CharterKeys.Charter, IniFormatting.Charter)]
	[DataRow(CharterKeys.Frets,	  CharterKeys.Frets,   IniFormatting.Frets)]
	[DataRow(CharterKeys.All,	  CharterKeys.Charter, IniFormatting.Charter)]
	[DataRow(CharterKeys.All,	  CharterKeys.Frets,   IniFormatting.Frets)]
	public void Remove_IniCharter_RemovesFlag(CharterKeys original, CharterKeys removed, string keyString)
	{
		CharterKeys expected = original.RemoveFlags(removed);

		Metadata metadata = new()
		{
			Charter    = new() { Name = "TheBoxyBear" },
			Formatting = new() { CharterKeys = original }
		};

		metadata.Remove(FileType.Ini, keyString);

		Assert.AreEqual(expected, metadata.Formatting.CharterKeys.Value);
	}

	[TestMethod, TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Remove)), TestCategory(nameof(Metadata.Charter))]
	[DataRow(CharterKeys.None,	  IniFormatting.Charter, true)]
	[DataRow(CharterKeys.None,	  IniFormatting.Frets,	 true)]
	[DataRow(CharterKeys.Charter, IniFormatting.Charter, true)]
	[DataRow(CharterKeys.Frets,	  IniFormatting.Frets,	 true)]
	[DataRow(CharterKeys.All,	  IniFormatting.Charter, false)]
	[DataRow(CharterKeys.All,	  IniFormatting.Frets,	 false)]
	public void Remove_IniCharter_RemovesValue(CharterKeys key, string keyString, bool expectedRemoved)
	{
		Metadata metadata = new()
		{
			Charter    = new() { Name = "TheBoxyBear" },
			Formatting = new() { CharterKeys = key }
		};

		metadata.Remove(FileType.Ini, keyString);

		Assert.AreEqual(expectedRemoved, metadata.Charter.Name is null);
	}

	[TestMethod, TestCategory(nameof(FileType.Ini))]
	[TestCategory(nameof(Metadata.Contains)), TestCategory(nameof(Metadata.Charter))]
	[DataRow(CharterKeys.Charter, IniFormatting.Charter, true)]
	[DataRow(CharterKeys.Frets,	  IniFormatting.Frets,	 true)]
	[DataRow(CharterKeys.All,	  IniFormatting.Charter, true)]
	[DataRow(CharterKeys.All,	  IniFormatting.Frets,	 true)]
	[DataRow(CharterKeys.Charter, IniFormatting.Frets,	 false)]
	[DataRow(CharterKeys.Frets,	  IniFormatting.Charter, false)]
	[DataRow(CharterKeys.None,	  IniFormatting.Charter, false)]
	[DataRow(CharterKeys.None,	  IniFormatting.Frets,	 false)]
	public void Contains_IniCharterMatch_Maps(CharterKeys key, string keyString, bool expected)
	{
		Metadata metadata = new()
		{
			Charter    = new() { Name = "TheBoxyBear" },
			Formatting = new() { CharterKeys = key }
		};

		Assert.AreEqual(expected, metadata.Contains(FileType.Ini, keyString));
	}
	#endregion
	#endregion

	#region Unidentified
	[TestMethod, TestCategory(nameof(Metadata.Get)), TestCategory(nameof(Metadata.Set))]
	[TestCategory(nameof(FileType.Chart)), TestCategory(nameof(FileType.Ini))]
	[DataRow(FileType.Chart), DataRow(FileType.Ini)]
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
	[TestCategory(nameof(FileType.Chart)), TestCategory(nameof(FileType.Ini))]
	[DataRow(FileType.Chart), DataRow(FileType.Ini)]
	public void Remove_Unidentified_Maps(FileType fileType)
	{
		const string key = "LoremIpsum";

		Metadata metadata = new();
		metadata.Set(fileType, key, "Lorem ipsum");
		metadata.Remove(fileType, key);

		Assert.IsNull(metadata.Get(fileType, key));
	}

	[TestMethod, TestCategory(nameof(Metadata.Contains))]
	[TestCategory(nameof(FileType.Chart)), TestCategory(nameof(FileType.Ini))]
	[DataRow(FileType.Chart), DataRow(FileType.Ini)]
	public void Contains_Unidentified_Maps(FileType fileType)
	{
		const string key = "LoremIpsum";

		Metadata metadata = new();
		metadata.Set(fileType, key, "Lorem ipsum");

		Assert.IsTrue(metadata.Contains(fileType, key));
	}
	#endregion
}
