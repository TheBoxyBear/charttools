namespace ChartTools;

/// <summary>
/// Marker that alters the time signature
/// </summary>
public class TimeSignature : TrackObjectBase
{
    /// <summary>
    /// Value of a beat
    /// </summary>
    public byte Numerator { get; set; }
    /// <summary>
    /// Beats per measure
    /// </summary>
    public byte Denominator { get; set; }

    /// <summary>
    /// Creates an instance of a 4/4 <see cref="TimeSignature"/>.
    /// </summary>
    public TimeSignature() : this(0) { }

    /// <inheritdoc cref="TimeSignature()"/>
    /// <param name="position">Value of <see cref="TrackObjectBase.Position"/></param>
    public TimeSignature(uint position) : base(position) => Numerator = Denominator = 4;

    /// <summary>
    /// Creates an instance of <see cref="TimeSignature"/>.
    /// </summary>
    /// <inheritdoc cref="TimeSignature(uint)"/>
    /// <param name="numerator">Value of <see cref="Numerator"/></param>
    /// <param name="denominator">Value of <see cref="Denominator"/></param>
    public TimeSignature(uint position, byte numerator, byte denominator) : base(position)
    {
        Numerator = numerator;
        Denominator = denominator;
    }
}
