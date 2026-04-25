using Lane  = ChartTools.StandardLane;
using Note  = ChartTools.LaneNote<ChartTools.StandardLane>;
using Proxy = ChartTools.NoteProxy<ChartTools.LaneNote<ChartTools.StandardLane>, ChartTools.StandardLane>;
using NoteCollection = ChartTools.LaneNoteCollection<ChartTools.LaneNote<ChartTools.StandardLane>, ChartTools.StandardLane>;

namespace ChartTools.Tests.Notes;

[TestClass]
public class NoteProxyTests
{
	[TestMethod, TestCategory("Ctor"), TestCategory(nameof(Exception))]
	public void Ctor_InvalidLane_Throws()
		=> Assert.Throws<UndefinedEnumException>(
			static () => new Proxy((Lane)10, []));

	[TestMethod, TestCategory("Ctor"), TestCategory(nameof(Exception))]
	public void Ctor_NullCollection_Throws()
		=> Assert.Throws<ArgumentNullException>(
			static () => new Proxy(Lane.Green, null!));

	[TestMethod, TestCategory(nameof(Proxy.Get))]
	public void Get_Match_ReturnsNote()
	{
		const Lane lane = Lane.Green;

		NoteCollection collection = [new Note(lane)];
		Proxy proxy = new(lane, collection);

		Note? note = proxy.Get();

		Assert.IsNotNull(note);
		Assert.AreEqual(lane, note.Value.Lane);
	}

	[TestMethod, TestCategory(nameof(Proxy.Get))]
	public void Get_NoMatch_ReturnsNull()
	{
		Proxy proxy = new(Lane.Green, []);
		Note? note  = proxy.Get();

		Assert.IsNull(note);
	}

	[TestMethod, TestCategory(nameof(Proxy.AddOrSet))]
	public void Set_NoMatch_Adds()
	{
		const Lane lane    = Lane.Green;
		const uint sustain = 100;

		NoteCollection collection = [];
		Proxy proxy = new(lane, collection);
		Note  note  = new(lane) { Sustain = sustain };

		proxy.AddOrSet(in note);

		Assert.AreEqual(1, collection.Count);

		ref readonly Note added = ref collection.AsSpan()[0];

		Assert.AreEqual(lane, added.Lane);
		Assert.AreEqual(sustain, added.Sustain);
	}

	[TestMethod, TestCategory(nameof(Proxy.AddOrSet))]
	public void Set_Match_Replaces()
	{
		const Lane lane    = Lane.Green;
		const uint sustain = 100;

		Note note = new(lane);
		NoteCollection collection = [note];
		Proxy proxy = new(lane, collection);

		proxy.AddOrSet(note with { Sustain = sustain});

		Assert.AreEqual(1, collection.Count);

		ref readonly Note added = ref collection.AsSpan()[0];

		Assert.AreEqual(lane, added.Lane);
		Assert.AreEqual(sustain, added.Sustain);
	}
}
