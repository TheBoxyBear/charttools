namespace ChartTools;

/// <summary>
/// Set of notes played simultaneously
/// </summary>
public abstract class Chord<TNote, TNoteEnum> : Chord where TNote : Note where TNoteEnum : struct, System.Enum
{
    protected abstract bool OpenExclusivity { get; }

    /// <summary>
    /// Notes in the <see cref="Chord{TNote}"/>
    /// </summary>
    public NoteCollection<TNote, TNoteEnum> Notes { get; init; }

    /// <inheritdoc cref="Chord(uint)" path="/param"/>
    protected Chord(uint position) : base(position) => Notes = new(OpenExclusivity);

    public override IEnumerable<Note> GetNotes() => Notes;
}
