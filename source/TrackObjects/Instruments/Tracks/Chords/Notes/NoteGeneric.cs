namespace ChartTools
{
    public class Note<TFretEnum> : Note where TFretEnum : struct, Enum
    {
        public TFretEnum Fret => (TFretEnum)(object)NoteIndex;

        public Note(TFretEnum note) : base(Convert.ToByte(note))
        {
            if (!Enum.IsDefined(note))
                throw new ArgumentException($"Note value is not defined.", nameof(note));
        }
    }
}
