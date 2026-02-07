namespace ChartTools.Tests.Notes;

[TestClass]
public class NoteTests
{
	[TestMethod, TestCategory("Ctor"), TestCategory(nameof(Exception))]
	public void Ctor_InvalidLane_Throws()
	{
		Assert.Throws<UndefinedEnumException>(static () => new LaneNote<StandardLane>((StandardLane)10));
		Assert.Throws<UndefinedEnumException>(static () => new LaneNote<GHLLane>((GHLLane)10));
		Assert.Throws<UndefinedEnumException>(static () => new DrumsNote((DrumsLane)10));
	}

	[TestMethod, TestCategory("Init"), TestCategory(nameof(Exception))]
	public void Init_InvalidLane_Throws()
	{
		Assert.Throws<UndefinedEnumException>(static () => new LaneNote<StandardLane>() { Lane = (StandardLane)10 });
		Assert.Throws<UndefinedEnumException>(static () => new LaneNote<GHLLane>() { Lane = (GHLLane)10 });
		Assert.Throws<UndefinedEnumException>(static () => new DrumsNote() { Lane = (DrumsLane)10 });
	}

	[TestMethod, TestCategory("Cast"), TestCategory(nameof(Exception))]
	public void Cast_Long_Throws()
	{
		Assert.Throws<InvalidCastException>(static () => (ILongObject)(object)new LaneNote<StandardLane>());
		Assert.Throws<InvalidCastException>(static () => (ILongObject)(object)new LaneNote<GHLLane>());
		Assert.Throws<InvalidCastException>(static () => (ILongObject)(object)new DrumsNote());
	}

	[TestMethod, TestCategory(nameof(INote.Index))]
	[DataRow((byte)1)]
	[DataRow((byte)2)]
	[DataRow((byte)3)]
	[DataRow((byte)4)]
	[DataRow((byte)5)]
	public void Index_Maps(byte index)
	{
		LaneNote<StandardLane> standard = new() { Lane = (StandardLane)index };
		Assert.AreEqual(index, standard.Index);

		LaneNote<GHLLane> ghl = new() { Lane = (GHLLane)index };
		Assert.AreEqual(index, standard.Index);

		DrumsNote drums = new() { Lane = (DrumsLane)index };
		Assert.AreEqual(index, standard.Index);
	}

	#region IsCymbal
	private static IEnumerable<object[]> CymbalLanes =>
		[[DrumsLane.Kick], [DrumsLane.Green4Lane_Orange5Lane], [DrumsLane.Yellow], [DrumsLane.Blue]];

	[TestMethod, TestCategory(nameof(DrumsNote.IsCymbal)), TestCategory(nameof(Exception))]
	[DataRow(DrumsLane.Red), DataRow(DrumsLane.Green5Lane)]
	public void IsCymbal_CtorInvalidLane_Throws(DrumsLane lane)
		=> Assert.Throws<InvalidOperationException>(
			() => new DrumsNote(lane) { IsCymbal = true });

	[TestMethod, TestCategory(nameof(DrumsNote.IsCymbal)), TestCategory(nameof(Exception))]
	[DynamicData(nameof(CymbalLanes))]
	public void IsCymbal_CtorValidLane_NoThrows(DrumsLane lane)
		=> _ = new DrumsNote(lane) { IsCymbal = true };

	[TestMethod, TestCategory(nameof(DrumsNote.IsCymbal)), TestCategory(nameof(Exception))]
	[DataRow(DrumsLane.Red), DataRow(DrumsLane.Green5Lane)]
	public void IsCymbal_InitInvalidLane_Throws(DrumsLane lane)
		=> Assert.Throws<InvalidOperationException>(
			() => new DrumsNote()
			{
				Lane     = lane,
				IsCymbal = true
			});

	[TestMethod, TestCategory(nameof(DrumsNote.IsCymbal)), TestCategory(nameof(Exception))]
	[DynamicData(nameof(CymbalLanes))]
	public void IsCymbal_InitValidLane_NoThrows(DrumsLane lane)
		=> _ = new DrumsNote()
		{
			Lane     = lane,
			IsCymbal = true
		};

	[TestMethod, TestCategory(nameof(DrumsNote.IsCymbal)), TestCategory(nameof(Exception))]
	[DataRow(DrumsLane.Red), DataRow(DrumsLane.Green5Lane)]
	public void IsCymbal_PostInitInvalidLane_Throws(DrumsLane lane)
		=> Assert.Throws<InvalidOperationException>(
			() => new DrumsNote()
			{
				IsCymbal = true,
				Lane     = lane
			});

	[TestMethod, TestCategory(nameof(DrumsNote.IsCymbal)), TestCategory(nameof(Exception))]
	[DynamicData(nameof(CymbalLanes))]
	public void IsCymbal_PostInitValidLane_NoThrows(DrumsLane lane)
		=> _ = new DrumsNote()
		{
			IsCymbal = true,
			Lane     = lane
		};
	#endregion
}
