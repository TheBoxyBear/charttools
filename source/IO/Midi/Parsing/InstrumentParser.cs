using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools.IO.Midi.Parsing
{
    internal abstract class InstrumentParser<TChord> : MidiParser where TChord : Chord
    {
        public InstrumentIdentity Instrument { get; }
        protected readonly Track<TChord>[] tracks = new Track<TChord>[4];

        public override Instrument<TChord> Result => GetResult(result);
        protected readonly Instrument<TChord> result;

        protected readonly InstrumentMapper<TChord> mapper;

        protected InstrumentParser(InstrumentIdentity instrument, InstrumentMapper<TChord> mapper, ReadingSession session) : base(session)
        {
            Instrument = instrument;
            result = new() { InstrumentIdentity = Instrument };
            this.mapper = mapper;
        }

        protected override void FinaliseParse()
        {
            foreach (var track in tracks)
                result.SetTrack(track);

            base.FinaliseParse();
        }
    }
}