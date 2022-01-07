namespace ChartTools
{
    /// <summary>
    /// Sequence of chords that gives star power if all the contained chords are played successfully
    /// </summary>
    public class SpecicalPhrase : TrackObject, System.IEquatable<SpecicalPhrase>, ILongTrackObject
    {
        /// <summary>
        /// Duration of the <see cref="SpecicalPhrase"/>
        /// </summary>
        public uint Length { get; set; }

        public SpecialPhraseType Type { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="SpecicalPhrase"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="TrackObject.Position"/></param>
        /// <param name="length">Value of <see cref="Length"/></param>
        public SpecicalPhrase(uint position, uint length = 0) : base(position) => Length = length;

        public override bool Equals(object? obj) => Equals(obj as SpecicalPhrase);
        public bool Equals(SpecicalPhrase? other) => throw new System.NotImplementedException();
    }
}
