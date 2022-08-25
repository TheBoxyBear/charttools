namespace ChartTools.IO
{
    internal interface IInstrumentAppliable<TChord> where TChord : Chord, new()
    {
        public void ApplyToInstrument(Instrument<TChord> instrument);
    }
}
