using ChartTools.IO.Chart.Configuration.Sessions;

namespace ChartTools.IO.Chart.Parsing;

internal abstract class VariableInstrumentTrackParser<TChord, TInstEnum>(Difficulty difficulty, TInstEnum instrument, ChartReadingSession session, string header)
    : TrackParser<TChord>(difficulty, session, header) where TChord : IChord, new() where TInstEnum : Enum
{
    public TInstEnum Instrument { get; } = instrument;
}
