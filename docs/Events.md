# Events

This guide will cover how to use events and their helpers.

### Class structure
Events are stored using `Event` as a base class, containing the position, event type and an optional argument. The type and argument can also be set simultaneously through the `EventData` property.

ChartTools distinguishes between global events (stored under `Song`) and local events (stored under `Track`) using the respective `GlobalEvent` and `LocalEvent` classed, both deriving from `Event`. This allows to know which types are a valid for a given event and for the classes to provide helper properties for some complex event types.

### Event helpers
Event types and arguments are stored as `string`, allowing for future-proofing and supporting custom events from sources that are not officially supported.

For event types which are supported, CharTools provides helpers in the form of string constants under the static `EventTypeHelper.Global` and `EventTypeHelper.Local` classes.

```c#
var globalEvent = new GlobalEvent(0, EventTypeHelper.Global.MusicStart, null);
```

Some event types are part of a category defined by a prefix to the type. Helpers are provided for these groups under the static `EventTypeHeaderHelper` class. Helper properties are also defined for groups from the supported games.

```c#
bool isCrowd = globalEvent.EventType.StartsWith(EventTypeHeaderHelper.Global.Crowd);
bool isCrowd2 = globalEvent.IsCrowdEvent;
```

Some event types can be modified using predefined arguments. For such values, helpers are provided under the static `EventArgumentHelper` class.

```c#
var globalEvent = new GlobalEvent(0, EventTypeHelper.Global.Lighting, EventArgumentHelper.Global.Lighting.Strobe);
```