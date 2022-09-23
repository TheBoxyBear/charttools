using System.Collections.Generic;

namespace ChartTools
{
    public interface IChord : ITrackObject
    {
        public IEnumerable<INote> Notes { get; }

        public INote CreateNote(byte index, uint sustain = 0);
    }
}
