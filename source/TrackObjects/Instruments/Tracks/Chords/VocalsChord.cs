using ChartTools.IO.Chart;
using ChartTools.Lyrics;

using System;
using System.Collections.Generic;

namespace ChartTools
{
    public class VocalsChord : Chord
    {
        public override byte ModifierKey { get; set; }

        private VocalsPitch _pitch;
        public VocalsPitch Pitch
        {
            get => _pitch;
            set => _pitch = Enum.IsDefined(value) ? value : throw CommonExceptions.GetUndefinedException(value);
        }
        public Syllable Syllable { get; set; }
        public uint Length { get; set; }

        protected override bool OpenExclusivity => false;

        public VocalsChord(uint position) : base(position) { }

        public override IEnumerable<Note> GetNotes() => throw new InvalidOperationException();

        internal override bool ChartModifierSupported() => true;

        internal override IEnumerable<string> GetChartData(ChartParser.WritingSession session, ICollection<byte> ignored)
        {
            throw new NotImplementedException();
        }
    }
}
