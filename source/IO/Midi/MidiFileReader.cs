using ChartTools.IO.Midi.Parsers;

using Melanchall.DryWetMidi.Core;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DryWetFile = Melanchall.DryWetMidi.Core.MidiFile;

namespace ChartTools.IO.Midi
{
    internal class MidiFileReader : FileReader<MidiEvent>
    {
        private readonly ReadingSettings settings;

        public override IEnumerable<MidiParser> Parsers => throw new NotImplementedException();

        public MidiFileReader(string path, ReadingSettings settings) : base(path)
        {
            this.settings = settings;
        }

        public override void Read()
        {
            throw new NotImplementedException();
        }
        public override Task ReadAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        public override void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
