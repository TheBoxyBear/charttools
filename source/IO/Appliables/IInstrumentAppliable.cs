namespace ChartTools.IO
{
    internal interface IInstrumentAppliable<TChord> where TChord : IChord
    {
        public void ApplyToInstrument(Instrument<TChord> instrument);
    }
}
