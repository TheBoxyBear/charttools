namespace ChartTools.IO
{
    internal interface IInstrumentAppliable<TChord> where TChord : Chord
    {
        public void ApplyToInstrument(Instrument<TChord> instrument);
    }
}
