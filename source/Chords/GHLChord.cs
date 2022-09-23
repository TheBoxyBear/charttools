﻿using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Formatting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously by a Guitar Hero Live instrument
    /// </summary>
    public class GHLChord : LaneChord<LaneNote<GHLLane>, GHLLane, GHLChordModifiers>
    {
        protected override bool OpenExclusivity => true;

        internal override GHLChordModifiers DefaultModifiers => GHLChordModifiers.None;
        internal override bool ChartSupportedModifiers => !Modifiers.HasFlag(GHLChordModifiers.ExplicitHopo);

        public GHLChord() : base() { }
        /// <inheritdoc cref="LaneChord{TNote, TLane, TModifier}(uint)"/>
        public GHLChord(uint position) : base(position) { }
        /// <inheritdoc cref="GHLChord(uint)"/>
        /// <param name="notes">Notes to add</param>
        public GHLChord(uint position, params LaneNote<GHLLane>[] notes) : base(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (var note in notes)
                Notes.Add(note);
        }
        /// <inheritdoc cref="GHLChord(uint, LaneNote{GHLLane}[])"/>
        public GHLChord(uint position, params GHLLane[] notes) : base(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (GHLLane note in notes)
                Notes.Add(new LaneNote<GHLLane>(note));
        }

        protected override IEnumerable<INote> GetNotes() => Notes;

        internal override IEnumerable<TrackObjectEntry> GetChartNoteData() => Notes.Select(note => ChartFormatting.NoteEntry(Position, note.Lane switch
        {
            var entries = Notes.Select(note => ChartFormatting.NoteEntry(Position, note.Lane switch
            {
                GHLLane.Open => 7,
                GHLLane.Black1 => 3,
                GHLLane.Black2 => 4,
                GHLLane.Black3 => 8,
                GHLLane.White1 => 0,
                GHLLane.White2 => 1,
                GHLLane.White3 => 2,
            }, note.Length));

        internal override IEnumerable<TrackObjectEntry> GetChartModifierData(LaneChord? previous, WritingSession session)
        {
            var isInvert = Modifiers.HasFlag(GHLChordModifiers.HopoInvert);

            if (Modifiers.HasFlag(GHLChordModifiers.ExplicitHopo) && (previous is null || previous.Position <= session.Formatting!.TrueHopoFrequency) != isInvert || isInvert)
                yield return ChartFormatting.NoteEntry(Position, 5, 0);
            if (Modifiers.HasFlag(GHLChordModifiers.Tap))
                yield return ChartFormatting.NoteEntry(Position, 6, 0);
        }
    }
}
