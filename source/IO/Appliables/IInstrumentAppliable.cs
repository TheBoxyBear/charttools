namespace ChartTools.IO
{
    internal interface IInstrumentAppliable<TChord> where TChord : IChord, new()
    {
        public void ApplyToInstrument(Instrument<TChord> instrument);
    }
}
