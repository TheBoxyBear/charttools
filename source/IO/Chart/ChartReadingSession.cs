using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartTools.IO.Chart
{
    internal class ChartReadingSession
    {
        public delegate bool IncludeNotePolicy(uint position, byte noteIndex, HashSet<byte> ignored);
        public delegate bool IncludeSyncTrackObjectPolicy(uint position, HashSet<uint> ignored, string typeName);

        public ReadingConfiguration Configuration { get; set; }

    }
}
