namespace ChartTools;

/// <summary>
/// Marker that alters the time signature
/// </summary>
public class TimeSignature : ITrackObject
{
    public uint Position { get; set; }

    /// <summary>
    /// Value of a beat
    /// </summary>
    public byte Numerator { get; set; }
    /// <summary>
    /// Beats per measure
    /// </summary>
    public byte Denominator { get; set; }

    /// <summary>
    /// Creates an instance of <see cref="TimeSignature"/>.
    /// </summary>
    /// <param name="position">Value of <see cref="Position"/></param>
    /// <param name="numerator">Value of <see cref="Numerator"/></param>
    /// <param name="denominator">Value of <see cref="Denominator"/></param>
    public TimeSignature(uint position, byte numerator, byte denominator)
    {
        Position = position;
        Numerator = numerator;
        Denominator = denominator;
    }
}
