using ChartTools.Collections;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Ini;
using ChartTools.Lyrics;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools
{
    /// <summary>
    /// Song playable in Clone Hero
    /// </summary>
    public class Song
    {
        #region Properties
        /// <summary>
        /// Set of about the song not unrelated to instruments, syncing or events
        /// </summary>
        public Metadata Metadata { get; set; } = new Metadata();
        /// <summary>
        /// Set of time signatures and tempo markers
        /// </summary>
        public SyncTrack SyncTrack { get; set; } = new SyncTrack();
        /// <summary>
        /// List of events common to all instruments
        /// </summary>
        public List<GlobalEvent> GlobalEvents { get; set; } = new List<GlobalEvent>();
        
        /// <summary>
        /// Set of drums tracks
        /// </summary>
        public Instrument<DrumsChord> Drums { get; set; }
        /// <summary>
        /// Set of Guitar Hero Live guitar tracks
        /// </summary>
        public Instrument<GHLChord> GHLGuitar { get; set; }
        /// <summary>
        /// Set of Guitar Hero Live bass tracks
        /// </summary>
        public Instrument<GHLChord> GHLBass { get; set; }
        /// <summary>
        /// Set of lead guitar tracks
        /// </summary>
        public Instrument<StandardChord> LeadGuitar { get; set; }
        /// <summary>
        /// Set of rythym guitar tracks
        /// </summary>
        public Instrument<StandardChord> RhythmGuitar { get; set; }
        /// <summary>
        /// Set of coop guitar tracks
        /// </summary>
        public Instrument<StandardChord> CoopGuitar { get; set; }
        /// <summary>
        /// Set of bass tracks
        /// </summary>
        public Instrument<StandardChord> Bass { get; set; }
        /// <summary>
        /// Set of keyboard tracks
        /// </summary>
        public Instrument<StandardChord> Keys { get; set; }
        #endregion

        /// <inheritdoc cref="ChartParser.ReadSong(string)"/>
        public static Song FromFile(string path) => ExtensionHandler.Read(path, (".chart", ChartParser.ReadSong), (".ini", (p) => new Song { Metadata = IniParser.Read(p) }));
        /// <inheritdoc cref="ChartParser.WriteSong(string, Song)"/>
        public void ToFile(string path) => ExtensionHandler.Write(path, this, (".chart", ChartParser.WriteSong));

        /// <summary>
        /// Retrieves the lyrics from the global events.
        /// </summary>
        public IEnumerable<Phrase> GetLyrics() => GlobalEvents is null ? null : GlobalEvents.GetLyrics();
        /// <summary>
        /// Replaces phrase and lyric events from <see cref="GlobalEvents"/> with the ones making up a set of <see cref="Phrase"/>.
        /// </summary>
        public void SetLyrics(IEnumerable<Phrase> phrases) => GlobalEvents = GlobalEvents.SetLyrics(phrases).ToList();
    }
}
