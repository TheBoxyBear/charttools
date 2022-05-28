using ChartTools.IO;
using ChartTools.IO.Chart;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools.Events
{
    /// <summary>
    /// Event common to all instruments
    /// </summary>
    public class GlobalEvent : Event
    {
        public bool IsBassistMovementEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.BassistMovement);
        public bool IsCrowdEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.Crowd);
        public bool IsDrummerMovementEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.DrummerMovement);
        public bool IsGuitaristMovementEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.GuitaristMovement) || EventType.StartsWith(EventTypeHeaderHelper.Global.GuitaristSolo);
        public bool IsGuitaristSoloEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.GuitaristSolo);
        public bool IsKeyboardMovementEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.KeyboardMovement);
        public bool IsLyricEvent => IsPhraseEvent || EventType == EventTypeHelper.Global.Lyric;
        public bool IsPhraseEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.Phrase);
        public bool IsSectionEvent => EventType is EventTypeHelper.Global.RB2CHSection or EventTypeHelper.Global.RB3Section;
        public bool IsSyncEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.Sync);
        public bool IsWailEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.GuitaristWail);

        /// <inheritdoc cref="Event(uint, string)"/>
        public GlobalEvent(uint position, string data) : base(position, data) { }
        /// <inheritdoc cref="Event(uint, string, string?)"/>
        public GlobalEvent(uint position, string type, string? argument = null) : base(position, type, argument) { }

        /// <summary>
        /// Reads global events from a file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        public static IEnumerable<GlobalEvent> FromFile(string path) => ExtensionHandler.Read<IEnumerable<GlobalEvent>>(path, (".chart", ChartFile.ReadGlobalEvents));
        /// <summary>
        /// Reads global events from a file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="FromFile(string)" path="/param[@name='path']"/></param>
        /// <param name="cancellationToken">Token to request cancellation</param>
        public static async Task<List<GlobalEvent>> FromFileAsync(string path, CancellationToken cancellationToken) => await ExtensionHandler.ReadAsync<List<GlobalEvent>>(path, (".chart", path => ChartFile.ReadGlobalEventsAsync(path, cancellationToken)));
    }
}
