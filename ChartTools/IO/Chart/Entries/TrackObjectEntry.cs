﻿namespace ChartTools.IO.Chart.Entries;

/// <summary>
/// Line of chart file data representing a <see cref="TrackObjectBase"/>
/// </summary>
internal readonly struct TrackObjectEntry : IReadOnlyTrackObject
{
    /// <summary>
    /// Value of <see cref="ITrackObject.Position"/>
    /// </summary>
    public uint Position { get; }
    /// <summary>
    /// Type code of <see cref="ITrackObject"/>
    /// </summary>
    public string Type { get; }
    /// <summary>
    /// Additional data
    /// </summary>
    public string Data { get; }

    /// <summary>
    /// Creates an instance of see<see cref="TrackObjectEntry"/>.
    /// </summary>
    /// <param name="line">Line in the file</param>
    /// <exception cref="LineException"/>
    public TrackObjectEntry(string line)
    {
        TextEntry entry = new(line);

        if (entry.Value is null)
            throw new LineException(line, new FormatException("Line has no object data."));

        string[] split = entry.Value.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

        if (split.Length < 2)
            throw new LineException(line, new EntryException());

        Type = split[0];
        Data = split[1];

        Position = ValueParser.ParseUint(entry.Key, "position");
    }
    public TrackObjectEntry(uint position, string type, string data)
    {
        Position = position;
        Type = type;
        Data = data;
    }

    public override string ToString() => ChartFormatting.Line(Position.ToString(), $"{Type} {Data}");
}
