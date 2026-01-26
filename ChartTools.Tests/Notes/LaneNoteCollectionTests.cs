using Microsoft.VisualStudio.TestTools.UnitTesting;

using Lane = ChartTools.StandardLane;
using Note = ChartTools.LaneNote<ChartTools.StandardLane>;
using NoteCollection = ChartTools.LaneNoteCollection<ChartTools.LaneNote<ChartTools.StandardLane>, ChartTools.StandardLane>;

namespace ChartTools.Tests.Notes;

[TestClass]
public class LaneNoteCollectionTests
{
	[TestMethod, TestCategory(nameof(NoteCollection.Add)), TestCategory(nameof(Exception))]
	public void Add_InvalidLane_Throws()
		=> Assert.ThrowsException<UndefinedEnumException>(
			static () => new NoteCollection().Add((Lane)10));

	[TestMethod, TestCategory(nameof(NoteCollection.AddRange)), TestCategory(nameof(Exception))]
	public void AddRange_InvalidLane_Throws()
		=> Assert.ThrowsException<UndefinedEnumException>(
			() => new NoteCollection().AddRange((Lane)10, (Lane)11));

	[TestMethod, TestCategory(nameof(NoteCollection.Add))]
	public void Add_Lane_Adds()
	{
		NoteCollection collection = [];
		collection.Add(Lane.Green);

		Assert.AreEqual(1, collection.Count);
	}

	[TestMethod, TestCategory(nameof(NoteCollection.AddRange))]
	public void AddRange_Lane_Adds()
	{
		const Lane
			first  = Lane.Green,
			second = Lane.Red;

		NoteCollection collection = [];
		collection.AddRange(first, second);

		Assert.IsTrue(collection.Contains(first));
		Assert.IsTrue(collection.Contains(new Note(first)));

		Assert.IsTrue(collection.Contains(second));
		Assert.IsTrue(collection.Contains(new Note(second)));
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

	[TestMethod, TestCategory(nameof(NoteCollection.AddRange))]
	public void AddRange_Note_Adds()
	{
		const Lane
			first  = Lane.Green,
			second = Lane.Red;

		NoteCollection collection = [];
		collection.AddRange(new Note(first), new Note(second));

		ref readonly Note firstAdded  = ref collection.AsSpan()[0];
		ref readonly Note secondAdded = ref collection.AsSpan()[1];

		Assert.AreEqual(2, collection.Count);
		Assert.AreEqual(first, firstAdded.Lane);
		Assert.AreEqual(second, secondAdded.Lane);
	}

	[TestMethod, TestCategory("Init")]
	public void Init_Note_Adds()
	{
		const Lane lane = Lane.Green;

		NoteCollection collection = [ new Note(lane) ];

		ref readonly Note added = ref collection.AsSpan()[0];

		Assert.AreEqual(1, collection.Count);
		Assert.AreEqual(lane, added.Lane);
	}

	[TestMethod, TestCategory("InitRange")]
	public void InitRange_Note_Adds()
	{
		const Lane
			first  = Lane.Green,
			second = Lane.Red;

		NoteCollection collection = [ new Note(first), new Note(second) ];

		ref readonly Note firstAdded  = ref collection.AsSpan()[0];
		ref readonly Note secondAdded = ref collection.AsSpan()[1];

		Assert.AreEqual(2, collection.Count);
		Assert.AreEqual(first, firstAdded.Lane);
		Assert.AreEqual(second, secondAdded.Lane);
	}

	[TestMethod, TestCategory(nameof(NoteCollection.Add))]
	public void Add_ExistingLane_Replaces()
	{
		const Lane lane = Lane.Green;
		const uint sustain = 100;

		NoteCollection collection = [ new Note(lane) { Sustain = sustain } ];
		collection.Add(lane);

		Assert.AreEqual(1, collection.Count);

		ref readonly Note replaced = ref collection.AsSpan()[0];

		Assert.AreEqual(lane, replaced.Lane);
		Assert.AreNotEqual(sustain, replaced.Sustain);
	}

	[TestMethod, TestCategory(nameof(NoteCollection.AddRange))]
	public void AddRange_ExistingLane_Replaces()
	{
		const Lane
			firstLane  = Lane.Green,
			secondLane = Lane.Red;

		const uint sustain = 100;

		NoteCollection collection =
		[
			new Note(firstLane) { Sustain = sustain },
			new Note(secondLane) { Sustain = sustain }
		];

		collection.AddRange(firstLane, secondLane);

		Assert.AreEqual(2, collection.Count);

		AssertReplaced(collection.AsSpan()[0], firstLane);
		AssertReplaced(collection.AsSpan()[1], secondLane);

		static void AssertReplaced(in Note replaced, Lane lane)
		{
			Assert.AreEqual(lane, replaced.Lane);
			Assert.AreNotEqual(sustain, replaced.Sustain);
		}
	}

	[TestMethod, TestCategory(nameof(NoteCollection.Add))]
	public void Add_ExistingNote_Replaces()
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

	[TestMethod, TestCategory(nameof(NoteCollection.AddRange))]
	public void AddRange_ExistingNote_Replaces()
	{
		const Lane
			firstLane  = Lane.Green,
			secondLane = Lane.Red;

		const uint sustain = 100;

		NoteCollection collection =
		[
			new Note(firstLane) { Sustain = sustain },
			new Note(secondLane) { Sustain = sustain }
		];

		collection.AddRange(new Note(firstLane), new Note(secondLane));

		Assert.AreEqual(2, collection.Count);

		AssertReplaced(collection.AsSpan()[0], firstLane);
		AssertReplaced(collection.AsSpan()[1], secondLane);

		static void AssertReplaced(in Note replaced, Lane lane)
		{
			Assert.AreEqual(lane, replaced.Lane);
			Assert.AreNotEqual(sustain, replaced.Sustain);
		}
	}

	[TestMethod, TestCategory(nameof(NoteCollection.Clear))]
	public void Clear_Empties()
	{
		NoteCollection collection = [ new Note(Lane.Green) ];
		collection.Clear();

		Assert.AreEqual(0, collection.Count);
	}

	[TestMethod, TestCategory(nameof(NoteCollection.Contains))]
	public void Contains_Lane_Finds()
	{
		const Lane lane = Lane.Green;

		NoteCollection collection = [];
		collection.Add(lane);

		Assert.IsTrue(collection.Contains(lane));
		Assert.IsTrue(collection.Contains(new Note(lane)));
	}

	[TestMethod, TestCategory(nameof(NoteCollection.Contains))]
	public void Contains_Note_Finds()
	{
		const Lane lane = Lane.Green;

		NoteCollection collection = [ new Note(lane) ];

		Assert.IsTrue(collection.Contains(lane));
		Assert.IsTrue(collection.Contains(new Note(lane)));
	}

	[TestMethod, TestCategory(nameof(NoteCollection.Remove)), TestCategory(nameof(Exception))]
	public void Remove_InvalidLane_Throws()
		=> Assert.ThrowsException<UndefinedEnumException>(
			static () => new NoteCollection().Remove((Lane)10));

	[TestMethod, TestCategory("Indexer"), TestCategory(nameof(Exception))]
	public void Indexer_Get_InvalidLane_Throws()
		=> Assert.ThrowsException<UndefinedEnumException>(
			static () => _ = new NoteCollection()[(Lane)10]);
}
