using Lane = ChartTools.StandardLane;
using Note = ChartTools.LaneNote<ChartTools.StandardLane>;
using NoteCollection = ChartTools.LaneNoteCollection<ChartTools.LaneNote<ChartTools.StandardLane>, ChartTools.StandardLane>;

namespace ChartTools.Tests.Notes;

[TestClass]
public class LaneNoteCollectionTests
{
	#region Add
	[TestMethod, TestCategory(nameof(NoteCollection.Add)), TestCategory(nameof(Exception))]
	public void Add_InvalidLane_Throws()
		=> Assert.Throws<UndefinedEnumException>(
			static () => new NoteCollection().Add((Lane)10));

	[TestMethod, TestCategory(nameof(NoteCollection.Add))]
	public void Add_Lane_Adds()
	{
		NoteCollection collection = [];
		collection.Add(Lane.Green);

		Assert.AreEqual(1, collection.Count);
	}

	[TestMethod, TestCategory(nameof(NoteCollection.Add))]
	public void Add_Note_Adds()
	{
		const Lane lane = Lane.Green;

		NoteCollection collection = [];
		collection.Add(new Note(lane));

		ref readonly Note added = ref collection.AsSpan()[0];

		Assert.AreEqual(1, collection.Count);
		Assert.AreEqual(lane, added.Lane);
	}

	[TestMethod, TestCategory(nameof(NoteCollection.Add))]
	public void Add_LaneMatch_Replaces()
	{
		const Lane lane = Lane.Green;
		const uint sustain = 100;

		NoteCollection collection = [new Note(lane) { Sustain = sustain }];
		collection.Add(lane);

		Assert.AreEqual(1, collection.Count);

		ref readonly Note replaced = ref collection.AsSpan()[0];

		Assert.AreEqual(lane, replaced.Lane);
		Assert.AreNotEqual(sustain, replaced.Sustain);
	}

	[TestMethod, TestCategory(nameof(NoteCollection.Add))]
	public void Add_NoteMatch_Replaces()
	{
		const Lane lane    = Lane.Green;
		const uint sustain = 100;

		NoteCollection collection = [new Note(lane) { Sustain = sustain }];
		collection.Add(new Note(lane));

		Assert.AreEqual(1, collection.Count);

		ref readonly Note replaced = ref collection.AsSpan()[0];

		Assert.AreEqual(lane, replaced.Lane);
		Assert.AreNotEqual(sustain, replaced.Sustain);
	}
	#endregion

	#region AddRange
	[TestMethod, TestCategory(nameof(NoteCollection.AddRange)), TestCategory(nameof(Exception))]
	public void AddRange_InvalidLane_Throws()
		=> Assert.Throws<UndefinedEnumException>(
			() => new NoteCollection().AddRange((Lane)10, (Lane)11));

	[TestMethod, TestCategory(nameof(NoteCollection.AddRange))]
	public void AddRange_Lane_Adds()
	{
		ReadOnlySpan<Lane> lanes = [Lane.Green, Lane.Red];

		NoteCollection collection = [];
		collection.AddRange(lanes[0], lanes[1]);

		foreach (ref readonly Lane lane in lanes)
		{
			Assert.IsTrue(collection.Contains(lane));
			Assert.IsTrue(collection.Contains(new Note(lane)));
		}
	}

	[TestMethod, TestCategory(nameof(NoteCollection.AddRange))]
	public void AddRange_Note_Adds()
	{
		ReadOnlySpan<Lane> lanes = [Lane.Green, Lane.Red];

		NoteCollection collection = [];
		collection.AddRange(new Note(lanes[0]), new Note(lanes[1]));

		Assert.AreEqual(lanes.Length, collection.Count);

		ReadOnlySpan<Note> added = collection.AsSpan()[..lanes.Length];

		for (int i = 0; i < lanes.Length; i++)
			Assert.AreEqual(lanes[i], added[i].Lane);
	}

	[TestMethod, TestCategory(nameof(NoteCollection.AddRange))]
	public void AddRange_LaneMatchReplaces()
	{
		ReadOnlySpan<Lane> lanes = [Lane.Green, Lane.Red];

		const uint sustain = 100;

		NoteCollection collection =
		[
			new Note(lanes[0]) { Sustain = sustain },
			new Note(lanes[1]) { Sustain = sustain }
		];

		collection.AddRange(lanes[0], lanes[1]);

		Assert.AreEqual(lanes.Length, collection.Count);

		ReadOnlySpan<Note> replaced = collection.AsSpan()[..lanes.Length];

		for (int i = 0; i < lanes.Length; i++)
		{
			Assert.AreEqual(lanes[i], replaced[i].Lane);
			Assert.AreNotEqual(sustain, replaced[i].Sustain);
		}
	}

	[TestMethod, TestCategory(nameof(NoteCollection.AddRange))]
	public void AddRange_NoteMatch_Replaces()
	{
		ReadOnlySpan<Lane> lanes = [Lane.Green, Lane.Red];

		const uint sustain = 100;

		NoteCollection collection =
		[
			new Note(lanes[0]) { Sustain = sustain },
			new Note(lanes[1]) { Sustain = sustain }
		];

		collection.AddRange(new Note(lanes[0]), new Note(lanes[1]));

		Assert.AreEqual(lanes.Length, collection.Count);

		ReadOnlySpan<Note> replaced = collection.AsSpan()[..lanes.Length];

		for (int i = 0; i < lanes.Length; i++)
		{
			Assert.AreEqual(lanes[i], replaced[i].Lane);
			Assert.AreNotEqual(sustain, replaced[i].Sustain);
		}
	}
	#endregion

	#region Init
	[TestMethod, TestCategory("Init")]
	public void Init_Note_Adds()
	{
		const Lane lane    = Lane.Green;
		const uint sustain = 100;

		NoteCollection collection = [new Note(lane) { Sustain = sustain }];

		ref readonly Note added = ref collection.AsSpan()[0];

		Assert.AreEqual(1, collection.Count);
		Assert.AreEqual(lane, added.Lane);
		Assert.AreEqual(sustain, added.Sustain);
	}

	[TestMethod, TestCategory("InitRange")]
	public void InitRange_Note_Adds()
	{
		ReadOnlySpan<Lane> lanes = [Lane.Green, Lane.Red];
		const uint sustain = 100;

		NoteCollection collection =
		[
			new Note(lanes[0]) { Sustain = sustain },
			new Note(lanes[1]) { Sustain = sustain }
		];

		ReadOnlySpan<Note> addedNotes = collection.AsSpan()[..lanes.Length];

		Assert.AreEqual(lanes.Length, collection.Count);

		for (int i = 0; i < lanes.Length; i++)
		{
			Assert.AreEqual(lanes[i], addedNotes[i].Lane);
			Assert.AreEqual(sustain, addedNotes[i].Sustain);
		}
	}
	#endregion

	[TestMethod, TestCategory(nameof(NoteCollection.Clear))]
	public void Clear_Empties()
	{
		NoteCollection collection = [new Note(Lane.Green)];
		collection.Clear();

		Assert.AreEqual(0, collection.Count);
	}

	#region Contains
	[TestMethod, TestCategory(nameof(NoteCollection.Contains)), TestCategory(nameof(Exception))]
	public void Contains_InvalidLane_Throws()
		=> Assert.Throws<UndefinedEnumException>(
			() => new NoteCollection().Contains((Lane)10));

	[TestMethod, TestCategory(nameof(NoteCollection.Contains))]
	public void Contains_MatchLane_ReturnsNote()
	{
		const Lane lane = Lane.Green;

		NoteCollection collection = [];
		collection.Add(lane);

		Assert.IsTrue(collection.Contains(lane));
		Assert.IsTrue(collection.Contains(new Note(lane)));
	}

	[TestMethod, TestCategory(nameof(NoteCollection.Contains))]
	public void Contains_MatchNote_ReturnsNote()
	{
		const Lane lane = Lane.Green;

		NoteCollection collection = [new Note(lane)];

		Assert.IsTrue(collection.Contains(lane));
		Assert.IsTrue(collection.Contains(new Note(lane)));
	}
	#endregion

	#region Remove
	[TestMethod, TestCategory(nameof(NoteCollection.Remove)), TestCategory(nameof(Exception))]
	public void Remove_InvalidLane_Throws()
		=> Assert.Throws<UndefinedEnumException>(
			static () => new NoteCollection().Remove((Lane)10));

	[TestMethod, TestCategory(nameof(NoteCollection.Remove))]
	public void Remove_Match_RemovesReturnsTrue()
	{
		const Lane lane = Lane.Green;

		NoteCollection collection = [new Note(lane)];

		Assert.IsTrue(collection.Remove(Lane.Green));
		Assert.AreEqual(0, collection.Count);
	}

	[TestMethod, TestCategory(nameof(NoteCollection.Remove))]
	public void Remove_NoMatch_NoRemoveReturnsFalse()
	{
		const Lane lane = Lane.Green;

		NoteCollection collection = [new Note(lane)];

		Assert.IsFalse(collection.Remove(Lane.Red));
		Assert.AreEqual(1, collection.Count);
	}
	#endregion

	#region Indexer
	[TestMethod, TestCategory("Indexer"), TestCategory(nameof(Exception))]
	public void Indexer_InvalidLane_Throws()
		=> Assert.Throws<UndefinedEnumException>(
			static () => _ = new NoteCollection()[(Lane)10]);

	[TestMethod, TestCategory("Indexer")]
	public void Indexer_Match_ReturnsNote()
	{
		const Lane lane = Lane.Green;

		NoteCollection collection = [];
		collection.Add(lane);

		Note? note = collection[lane];

		Assert.IsNotNull(note);
		Assert.AreEqual(lane, note.Value.Lane);
	}

	[TestMethod, TestCategory("Indexer")]
	public void Indexer_NoMatch_ReturnsNull()
	{
		NoteCollection collection = [];
		Note? note = collection[Lane.Green];

		Assert.IsNull(note);
	}
	#endregion

	#region Proxy
	[TestMethod, TestCategory(nameof(NoteCollection.Proxy)), TestCategory(nameof(Exception))]
	public void Proxy_InvalidLane_Throws()
		=> Assert.Throws<UndefinedEnumException>(
			static () => new NoteCollection().Proxy((Lane)10));

	[TestMethod, TestCategory(nameof(NoteCollection.Proxy))]
	public void Proxy_NoMatch_ReturnsNull()
		=> Assert.IsNull(new NoteCollection().Proxy(Lane.Green));

	[TestMethod, TestCategory(nameof(NoteCollection.Proxy))]
	public void Proxy_Match_ReturnsProxy()
	{
		const Lane lane = Lane.Green;

		NoteCollection collection = [new Note(lane)];
		NoteProxy<Note, Lane>? proxy = collection.Proxy(lane);

		Assert.IsNotNull(proxy);
		Assert.AreEqual(collection, proxy.Value.Source);
		Assert.AreEqual(lane, proxy.Value.Lane.Value);
	}
	#endregion

	#region ProxyAll
	[TestMethod, TestCategory(nameof(NoteCollection.ProxyAll))]
	public void ProxyAll_Empty_ReturnsEmpty()
		=> Assert.IsEmpty(new NoteCollection().ProxyAll());

	[TestMethod, TestCategory(nameof(NoteCollection.ProxyAll))]
	public void ProxyAll_ReturnsProxies()
	{
		ReadOnlySpan<Lane> lanes = [Lane.Green, Lane.Red];

		NoteCollection collection = [new Note(Lane.Green), new Note(Lane.Red)];
		NoteProxy<Note, Lane>[] proxies = collection.ProxyAll();

		Assert.HasCount(lanes.Length, proxies);

		for (int i = 0; i < lanes.Length; i++)
		{
			Assert.AreEqual(collection, proxies[i].Source);
			Assert.AreEqual(lanes[i], proxies[i].Lane.Value);
		}
	}
	#endregion
}
