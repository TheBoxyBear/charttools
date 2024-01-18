# Events
Events are track objects with custom data that drive various elements of gameplay. This guide will cover how to use events as well as the helpers provided by ChartTools.

## Class structure
Events are stored using [Event](~/api/ChartTools.Events.Event.yml) as a base class, containing the position, event type and an optional argument. The type and argument can also be set simultaneously through the [EventData](~/api/ChartTools.Events.Event.yml#ChartTools_Events_Event_EventData) property.

ChartTools distinguishes between global events (stored under [Song](~/api/ChartTools.Song.yml)) and local events (stored under [Track](~/api/ChartTools.Track.yml)) using the respective [GlobalEvent](~/api/ChartTools.Events.GlobalEvent.yml) and [LocalEvent](~/api/ChartTools.Events.LocalEvent.yml) classes, both deriving from [Event](~/api/ChartTools.Events.Event.yml). This allows for better type safety and for the classes to provide helper properties for some complex event types.

## Event helpers
Event types and arguments are stored as `string`, allowing for future-proofing and supporting custom events from sources that are not officially supported.

For event types which are supported, CharTools provides helpers in the form of string constants under the static [EventTypeHelper.Global](~/api/ChartTools.Events.EventTypeHeaderHelper.Global.yml) and [EventTypeHelper.Local](~/api/ChartTools.Events.EventTypeHeaderHelper.Local.yml) classes. In a future version, usage details of helpers will be accessible from the documentation included with the assembly.

```c#
var globalEvent = new GlobalEvent(0, EventTypeHelper.Global.MusicStart, null);
```

Some event types are part of a category defined by a prefix to the type. Helpers are provided for these groups under the static [EventTypeHeaderHelper](~/api/ChartTools.Events.EventTypeHeaderHelper.yml) class. Helper properties are also defined for groups from supported sources.

```c#
bool isCrowd = globalEvent.EventType.StartsWith(EventTypeHeaderHelper.Global.Crowd);
bool isCrowd2 = globalEvent.IsCrowdEvent;
```

Some event types can be modified using predefined arguments. For such values, helpers are provided under the static [EventArgumentHelper](~/api/ChartTools.Events.EventArgumentHelper.html) class.

```c#
var globalEvent = new GlobalEvent(0, EventTypeHelper.Global.Lighting, EventArgumentHelper.Global.Lighting.Strobe);
```
