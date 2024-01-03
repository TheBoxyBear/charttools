using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Chart.Parsing;

internal abstract class VariableInstrumentTrackParser<TChord, TInstEnum> : TrackParser<TChord> where TChord : IChord, new() where TInstEnum : Enum
{
    public TInstEnum Instrument { get; }
    public VariableInstrumentTrackParser(Difficulty difficulty, TInstEnum instrument, ReadingSession session, string header) : base(difficulty, session, header) => Instrument = instrument;
}
