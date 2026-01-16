namespace ChartTools.IO.Components;

/// <summary>
/// Set of components to include in a read/write operation
/// </summary>
public record ComponentList()
{
	/// <summary>
	/// Creates a new <see cref="ComponentList"/> with only non-instrument components included.
	/// </summary>
	public static ComponentList Global() => new()
	{
		Metadata     = true,
		SyncTrack    = true,
		GlobalEvents = true
	};

	/// <summary>
	/// Creates a new <see cref="ComponentList"/> with only all components included.
	/// </summary>
	public static ComponentList Full() => new()
	{
		Metadata     = true,
		SyncTrack    = true,
		GlobalEvents = true,
		Vocals       = true,
		Instruments  = InstrumentComponentList.Full()
	};

	/// <summary>
	/// Include the <see cref="ChartTools.Meta"/>
	/// </summary>
	public bool Metadata { get; set; }

	/// <summary>
	/// Include the <see cref="ChartTools.SyncTrack"/>
	/// </summary>
	public bool SyncTrack { get; set; }

	/// <summary>
	/// Include the set of <see cref="ChartTools.Events.GlobalEvent"/>
	/// </summary>
	public bool GlobalEvents { get; set; }

	/// <summary>
	/// Include the <see cref="ChartTools.Lyrics.Vocals"/> track
	/// </summary>
	public bool Vocals { get; set; }

	/// <inheritdoc cref="InstrumentComponentList"/>
	public InstrumentComponentList Instruments { get; set; } = new();
}
