using ChartTools.IO.Chart;
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
    public class GHLChord : LaneChord<Note<GHLLane>, GHLLane, GHLChordModifier>
    {
        protected override bool OpenExclusivity => true;
        internal override bool ChartSupportedMoridier => !Modifiers.HasFlag(GHLChordModifier.ExplicitHopo);

        protected override GHLChordModifier DefaultModifiers => GHLChordModifier.None;

        public GHLChord() : base() { }
        /// <inheritdoc cref="LaneChord{TNote, TLane, TModifier}(uint)"/>
        public GHLChord(uint position) : base(position) { }
        /// <inheritdoc cref="GHLChord(uint)"/>
        /// <param name="notes">Notes to add</param>
        public GHLChord(uint position, params Note<GHLLane>[] notes) : base(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (Note<GHLLane> note in notes)
                Notes.Add(note);
        }
        /// <inheritdoc cref="GHLChord(uint, Note{GHLLane}[])"/>
        public GHLChord(uint position, params GHLLane[] notes) : base(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (GHLLane note in notes)
                Notes.Add(new Note<GHLLane>(note));
        }

        internal override IEnumerable<TrackObjectEntry> GetChartData(Chord? previous, bool modifiers, FormattingRules formatting)
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

            if (modifiers)
            {
                var isInvert = Modifiers.HasFlag(GHLChordModifier.HopoInvert);

                if (Modifiers.HasFlag(GHLChordModifier.ExplicitHopo) && (previous is null || previous.Position <= formatting.TrueHopoFrequency) != isInvert || isInvert)
                    entries = entries.Append(ChartFormatting.NoteEntry(Position, 5, 0));
                if (Modifiers.HasFlag(GHLChordModifier.Tap))
                    entries = entries.Append(ChartFormatting.NoteEntry(Position, 6, 0));
            }

            return entries;
        }
    }
}
