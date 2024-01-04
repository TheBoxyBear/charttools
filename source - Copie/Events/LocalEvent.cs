﻿namespace ChartTools.Events;

/// <summary>
/// Event specific to an instrument and difficulty
/// </summary>
public class LocalEvent : Event
{
    public bool IsSoloEvent => EventType is EventTypeHelper.Local.Solo or EventTypeHelper.Local.SoloEnd;
    public bool IsOwFaceEvent => EventType.StartsWith(EventTypeHeaderHelper.Local.OwFace);

    /// <inheritdoc cref="Event(uint, string)"/>
    public LocalEvent(uint position, string data) : base(position, data) { }
    /// <inheritdoc cref="Event(uint, string, string?)"/>
    public LocalEvent(uint position, string type, string? argument = null) : base(position, type, argument) { }

}
