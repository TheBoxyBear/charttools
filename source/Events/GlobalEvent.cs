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
        public bool IsSectionEvent => EventType is EventTypeHelper.Global.RB2CHSection or EventTypeHelper.Global.RB3Section;
        public bool IsLyricEvent => EventType is EventTypeHelper.Global.PhraseStart or EventTypeHelper.Global.PhraseEnd or EventTypeHelper.Global.Lyric;
        public bool IsCrowdEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.Crowd);
        public bool IsSyncEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.Sync);

        /// <inheritdoc cref="Event(uint, string)"/>
        public GlobalEvent(uint position, string data) : base(position, data) { }
        /// <inheritdoc cref="Event(uint, string, string?)"/>
        public GlobalEvent(uint position, string type, string? argument = null) : base(position, type, argument) { }

        /// <summary>
        /// Reads global events from a file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        public static IEnumerable<GlobalEvent> FromFile(string path) => ExtensionHandler.Read<IEnumerable<GlobalEvent>>(path, null, (".chart", (path, _) => ChartFile.ReadGlobalEvents(path)));
        /// <summary>
        /// Reads global events from a file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="FromFile(string)" path="/param[@name='path']"/></param>
        /// <param name="cancellationToken">Token to request cancellation</param>
        public static async Task<List<GlobalEvent>> FromFileAsync(string path, CancellationToken cancellationToken) => await ExtensionHandler.ReadAsync<List<GlobalEvent>>(path, cancellationToken, null, (".chart", (path, token, _) => ChartFile.ReadGlobalEventsAsync(path, token)));
    }
}
