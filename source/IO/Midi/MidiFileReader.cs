using ChartTools.Extensions.Collections;
using ChartTools.IO.Midi.Parsing;

using Melanchall.DryWetMidi.Core;

using System;
using System.Linq;
using System.Threading;

using DryWetFile = Melanchall.DryWetMidi.Core.MidiFile;

namespace ChartTools.IO.Midi
{
    internal class MidiFileReader : FileReader<MidiEvent, MidiParser>
    {
        public ReadingSettings? Settings { get; }
        public uint Resolution { get; private set; }

        public MidiFileReader(string path, Func<string, MidiParser?> parserGetter, ReadingSettings? settings) : base(path, parserGetter) => Settings = settings;

        protected override void ReadBase(bool async, CancellationToken cancellationToken)
        {
            ParserContentGroup? currentGroup = null;

            var file = DryWetFile.Read(Path, Settings);

            if (file.TimeDivision is not TicksPerQuarterNoteTimeDivision ticks)
                throw new Exception($"Time division must be of type {nameof(TicksPerQuarterNoteTimeDivision)}.");

            Resolution = (uint)ticks.TicksPerQuarterNote;

            foreach (var events in file.GetTrackChunks().Select(c => c.Events))
            {
                using var enumerator = events.GetEnumerator();
                enumerator.MoveNext();

                if (enumerator.Current is not SequenceTrackNameEvent header)
                {
                    // TODO Configuration
                    continue;
                }

                var parser = parserGetter(header.Text);


                if (parser is not null)
                {
                    var source = new DelayedEnumerableSource<MidiEvent>();

                    parserGroups.Add(currentGroup = new(parser, source));

                    if (async)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            currentGroup?.Source.EndAwait();
                            Dispose();

                            return;
                        }

                        parseTasks.Add(parser.StartAsyncParse(source.Enumerable));
                    }

                    while (enumerator.MoveNext())
                        currentGroup.Source.Add(enumerator.Current);

                    currentGroup.Source.EndAwait();
                }
            }
        }
    }
}
