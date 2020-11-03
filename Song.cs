using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Ini;
using ChartTools.Lyrics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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

        /// <summary>
        /// Gets property value for an <see cref="Instrument"/> from a <see cref="Instruments"/> <see langword="enum"/> value.
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns>Instance of <see cref="Instrument"/> from the <see cref="Song"/>.</returns>
        public Instrument GetInstrument(Instruments instrument) => instrument switch
        {
            Instruments.Drums => Drums,
            Instruments.GHLGuitar => GHLGuitar,
            Instruments.GHLBass => GHLBass,
            Instruments.LeadGuitar => LeadGuitar,
            Instruments.RhythmGuitar => RhythmGuitar,
            Instruments.CoopGuitar => CoopGuitar,
            Instruments.Bass => Bass,
            Instruments.Keys => Keys,
        };
        /// <summary>
        /// Gets property value for an <see cref="Instrument{TChord}"/> from a <see cref="GHLInstrument"/> <see langword="enum"/> value.
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="GHLChord"/> from the <see cref="Song"/>.</returns>
        public Instrument<GHLChord> GetInstrument(GHLInstrument instrument) => GetInstrument((Instruments)instrument) as Instrument<GHLChord>;
        /// <summary>
        /// Gets property value for an <see cref="Instrument{TChord}"/> from a <see cref="StandardInstrument"/> <see langword="enum"/> value.
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="StandardChord"/> from the <see cref="Song"/>.</returns>
        public Instrument<StandardChord> GetInstrument(StandardInstrument instrument) => GetInstrument((Instruments)instrument) as Instrument<StandardChord>;

        /// <inheritdoc cref="ChartParser.ReadSong(string)"/>
        public static Song FromFile(string path) => ExtensionHandler.Read(path, (".chart", ChartParser.ReadSong), (".ini", (p) => new Song { Metadata = IniParser.ReadMetadata(p) }));
        /// <inheritdoc cref="ChartParser.WriteSong(string, Song)"/>
        public void ToFile(string path) => ExtensionHandler.Write(path, this, (".chart", ChartParser.WriteSong));

        /// <summary>
        /// Reads the estimated instrument difficulties from a ini file.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="IOException"/>
        public void ReadDifficulties(string path) => ExtensionHandler.Read(path, (".ini", (p) => IniParser.ReadDifficulties(p, this)));
        /// <summary>
        /// Writes the estimated instrument difficulties to a ini file.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="IOException"/>
        public void WriteDifficulties(string path) => ExtensionHandler.Write<Song>(path, this, (".ini", IniParser.WriteDifficulties));

        /// <summary>
        /// Retrieves the lyrics from the global events.
        /// </summary>
        public IEnumerable<Phrase> GetLyrics() => GlobalEvents?.GetLyrics();
        /// <summary>
        /// Replaces phrase and lyric events from <see cref="GlobalEvents"/> with the ones making up a set of <see cref="Phrase"/>.
        /// </summary>
        public void SetLyrics(IEnumerable<Phrase> phrases) => GlobalEvents = GlobalEvents.SetLyrics(phrases).ToList();
    }
}
