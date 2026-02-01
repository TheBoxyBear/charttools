namespace ChartTools;

internal static class Validator
{
	/// <summary>
	/// Validates that an <see cref="Enum"/> value is defined.
	/// </summary>
	/// <exception cref="UndefinedEnumException"></exception>
	public static void ValidateEnum<T>(T value)
		where T : struct, Enum
	{
		if (!Enum.IsDefined(value))
			throw new UndefinedEnumException(value);
	}

    /// Validates that a track is attached to an instrument.
    /// </summary>
    /// <param name="track">Track to validate</param>
    public static void ValidateParentInstrument(Track track)
    {
        if (track.ParentInstrument is null)
            throw new DetachedTrackException(track);
    }
}
