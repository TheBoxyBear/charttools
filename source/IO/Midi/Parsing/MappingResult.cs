namespace ChartTools.IO.Midi.Parsing
{
    internal enum MappingType : byte { Note, Modifier, Special }

    internal ref struct MappingResult<TChord> where TChord : Chord
    {
        public Track<TChord>? Track { get; }
        public MappingType Type { get; }
        public byte Index { get; }

        public MappingResult(Track<TChord>? track, MappingType type, byte index)
        {
            Track = track;
            Type = type;
            Index = index;
        }
    }
}
