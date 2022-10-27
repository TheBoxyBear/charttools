using ChartTools.Exceptions;
using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Formatting;
using ChartTools.IO.Midi.Mapping;

using Melanchall.DryWetMidi.Core;

using System;
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
            var identity = Content.ParentInstrument!.InstrumentIdentity;
            var potentialHeaders = MidiFormatting.PotentialHeaders(Content.ParentInstrument!.InstrumentIdentity);

            if (!MidiFormatting.FindChunk(file.GetTrackChunks(), header => potentialHeaders.Any(h => h == header), out var header, out var chunkEvents))
                return Array.Empty<IEnumerable<IMidiEventMapping>>();

            var readSession = new ReadingSession(readConfig, formatting);
            var standardIdentity = (StandardInstrumentIdentity)identity;

            IReadInstrumentMapper mapper = header switch
            {
                MidiFormatting.GHGemsHeader => new GHGemsMapper(),
                MidiFormatting.LeadGuitarHeader or MidiFormatting.BassHeader => new GuitarBassMapper(standardIdentity),
                _ => throw new ImpossibleException($"Found header {header} not bound to a mapper.")
            };

            IEnumerable<NoteEventMapping> mappings = GetMappings(chunkEvents, mapper);

            // Get the format from the file and map to it
            if (mapper is GuitarBassMapper gbMapper)
            {
                mappings = mappings.ToArray(); // Resolve the enumeration to find the format

                var format = gbMapper.Format;

                if (format == MidiInstrumentOrigin.Unknown)
                    format = readSession.UncertainGuitarBassFormatProcedure(standardIdentity, format);

                mapper = new GuitarBassMapper(standardIdentity, format);
            }

            var newMappings = mapper switch
            {
                Track<StandardChord> standard => ((InstrumentMapper<StandardChord>)mapper).Map(standard),
                Track<GHLChord> ghl => ((InstrumentMapper<GHLChord>)mapper).Map(ghl),
                Track<DrumsChord> drums => ((InstrumentMapper<DrumsChord>)mapper).Map(drums)
            };

            return new IEnumerable<IMidiEventMapping>[]
            {
                mappings.Cast<INoteMapping>().Where(m => m.Difficulty != Content.Difficulty).Concat(newMappings.Cast<INoteMapping>())
            };

            static IEnumerable<NoteEventMapping> GetMappings(IEnumerator<MidiEvent> events, IReadInstrumentMapper mapper)
            {
                uint position = 0;

                while (events.MoveNext())
                {
                    position += (uint)events.Current.DeltaTime;

                    if (events.Current is NoteEvent e)
                        foreach (var mapping in mapper.Map(position, e))
                            yield return mapping;
                }
            }
        }
    }
}