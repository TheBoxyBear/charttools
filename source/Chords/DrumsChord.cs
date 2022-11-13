﻿using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Formatting;

namespace ChartTools;

/// <summary>
/// Set of notes played simultaneously by drums
/// </summary>
public class DrumsChord : LaneChord<DrumsNote, DrumsLane, DrumsChordModifiers>
{
    public override bool OpenExclusivity => false;

        protected override DrumsChordModifiers DefaultModifiers => DrumsChordModifiers.None;
        internal override bool ChartSupportedModifiers => true;

    public DrumsChord() : base() { }

    /// <inheritdoc cref="LaneChord(uint)"/>
    public DrumsChord(uint position) : base(position) { }
    /// <inheritdoc cref="DrumsChord(uint)"/>
    /// <param name="notes">Notes to add</param>
    public DrumsChord(uint position, params DrumsNote[] notes) : base(position)
    {
        if (notes is null)
            throw new ArgumentNullException(nameof(notes));

        foreach (DrumsNote note in notes)
            Notes.Add(note);
    }
    /// <inheritdoc cref="DrumsChord(uint, DrumsNote[])"/>
    public DrumsChord(uint position, params DrumsLane[] notes) : base(position)
    {
        if (notes is null)
            throw new ArgumentNullException(nameof(notes));

        foreach (DrumsLane note in notes)
            Notes.Add(new DrumsNote(note));
    }

    protected override IReadOnlyCollection<LaneNote> GetNotes() => Notes;

        internal override IEnumerable<TrackObjectEntry> GetChartData(LaneChord? previous, bool modifiers, FormattingRules formatting)
        {
            foreach (var note in Notes)
            {
                yield return ChartFormatting.NoteEntry(Position, note.Lane == DrumsLane.DoubleKick ? (byte)32 : note.Index, note.Sustain);

                if (note.IsCymbal)
                    yield return ChartFormatting.NoteEntry(Position, (byte)(note.Lane + 64), 0);
            }

            if (modifiers && Modifiers.HasFlag(DrumsChordModifiers.Flam))
                yield return ChartFormatting.NoteEntry(Position, 109, 0);
        }
    }
}
