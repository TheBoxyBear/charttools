using Melanchall.DryWetMidi.Core;
using System.Collections.Generic;

namespace ChartTools.IO.Midi.Mapping
{
    internal interface IReadInstrumentMapper
    {
        public IEnumerable<NoteEventMapping> Map(uint position, NoteEvent e);
    }
}
