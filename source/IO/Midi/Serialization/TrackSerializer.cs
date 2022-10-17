using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Formatting;
using ChartTools.IO.Midi.Mapping;
using ChartTools.IO.Midi.Parsing;

using Melanchall.DryWetMidi.Core;

using System.Collections.Generic;
using System.Linq;

using DryWetFile = Melanchall.DryWetMidi.Core.MidiFile;

namespace ChartTools.IO.Midi.Serialization
{
    internal class TrackSerializer : TrackObjectGroupSerializer<Track>
    {
        private readonly DryWetFile file;
        private readonly ReadingConfiguration readConfig;
        private readonly FormattingRules formatting;

        public TrackSerializer(string header, Track content, DryWetFile file, WritingSession session, ReadingConfiguration readConfig, FormattingRules formatting) : base(header, content, session)
        {
            this.file = file;
            this.readConfig = readConfig;
            this.formatting = formatting;
        }

        protected override IEnumerable<IMidiEventMapping>[] LaunchMappers()
        {
            string? header = null;
            IEnumerator<MidiEvent>? chunkEvents = null;

            if (Content is Track<StandardChord> standard)
            {
                var identity = standard.ParentInstrument!.InstrumentIdentity;
                var potentialHeaders = new List<string>();

                switch (identity)
                {
                    case InstrumentIdentity.LeadGuitar:
                        potentialHeaders.Add(MidiFormatting.GHGemsHeader);
                        potentialHeaders.Add(MidiFormatting.LeadGuitarHeader);
                        break;
                    case InstrumentIdentity.Bass:
                        potentialHeaders.Add(MidiFormatting.BassHeader);
                        break;
                }

                if (MidiFormatting.FindChunk(file.GetTrackChunks(), header => potentialHeaders.Any(h => h == header), out header, out chunkEvents))
                {
                    if (header is MidiFormatting.LeadGuitarHeader or MidiFormatting.BassHeader)
                    {
                        var readSession = new ReadingSession(readConfig, formatting);

                        var parser = new GuitarBassParser((StandardInstrumentIdentity)identity, readSession);
                    }
                }
            }
        }
    }
}
