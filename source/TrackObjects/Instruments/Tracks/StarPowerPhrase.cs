namespace ChartTools;

/// <summary>
/// Sequence of chords that gives star power if all the contained chords are played successfully
/// </summary>
public class StarPowerPhrase : TrackObject, System.IEquatable<StarPowerPhrase>
{
    /// <summary>
    /// Duration of the <see cref="StarPowerPhrase"/>
    /// </summary>
    public uint Length { get; set; }

    /// <summary>
    /// Creates an instance of <see cref="StarPowerPhrase"/>.
    /// </summary>
    /// <param name="position">Value of <see cref="TrackObject.Position"/></param>
    /// <param name="length">Value of <see cref="Length"/></param>
    public StarPowerPhrase(uint position, uint length = 0) : base(position) => Length = length;

    public override bool Equals(object? obj) => Equals(obj as StarPowerPhrase);
    public bool Equals(StarPowerPhrase? other) => throw new System.NotImplementedException();
}
