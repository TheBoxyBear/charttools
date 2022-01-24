namespace ChartTools.IO.Formatting
{
    /// <summary>
    /// Rules defined in song.ini that affect how the song data file is read and written
    /// </summary>
    /// <remarks>Property summaries provided by Nathan Hurst.</remarks>
    public class FormattingRules
    {
        public AlbumTrackKey AlbumTrackKey { get; set; }
        public CharterKey CharterKey { get; set; }
        public uint? Resolution { get; set; }
        public uint TrueResolution => Resolution ?? 480;

        /// <summary>
        /// Overrides the default sustain cutoff threshold with the specified number of ticks.
        /// </summary>

        public uint? SustainCutoff { get; set; }

        #region Hopo frequency
        /// <summary>
        /// Overrides the natural HOPO threshold with the specified number of ticks.
        /// </summary>
        [IniKey("hopo_frequency")]
        public uint? HopoFrequency { get; set; }
        /// <summary>
        /// (FoFiX) Overrides the natural HOPO threshold using numbers from 0 to 5.
        /// </summary>
        [IniKey("hopofreq")]
        public HopoFrequencyStep? HopoFrequencyStep { get; set; }
        /// <summary>
        /// (FoFiX) Overrides the natural HOPO threshold to be a 1/8th step.
        /// </summary>
        [IniKey("eighthnote_hopo")]
        public bool? ForceEightHopoFrequency { get; set; }

        public uint? TrueHopoFrequency
        {
            get
            {
                if (HopoFrequency is not null)
                    return HopoFrequency.Value;

                if (HopoFrequencyStep is not null)
                    return TrueResolution / (uint)(HopoFrequencyStep.Value switch
                    {
                        Formatting.HopoFrequencyStep.Fourth => 4,
                        Formatting.HopoFrequencyStep.Eight => 8,
                        Formatting.HopoFrequencyStep.Twelveth => 12,
                        Formatting.HopoFrequencyStep.Sixteenth => 16
                    });

                return ForceEightHopoFrequency is true ? TrueResolution / 8 : null;
            }
        }
        #endregion

        #region Star power
        /// <summary>
        /// Overrides the Star Power phrase MIDI note for .mid charts.
        /// </summary>
        [IniKey("multiplier_note")]
        public byte? MultiplierNote { get; set; }
        /// <summary>
        /// (PhaseShift) Overrides the Star Power phrase MIDI note for .mid charts.
        /// </summary>
        [IniKey("star_power_note")]
        public byte? StarPowerNote { get; set; }
        public byte? TrueStarPowerNote => StarPowerNote ?? MultiplierNote;
        #endregion

        #region SysEx
        /// <summary>
        /// (PhaseShift) Indicates if the chart uses SysEx events for sliders/tap notes.
        /// </summary>
        public bool? SysExSliders { get; set; }
        /// <summary>
        /// (PhaseShift) Indicates if the chart uses SysEx events for Drums Real hi-hat pedal control.
        /// </summary>
        public bool? SysExHighHat { get; set; }
        /// <summary>
        /// (PhaseShift) Indicates if the chart uses SysEx events for Drums Real rimshot hits.
        /// </summary>
        public bool? SysExRimshot { get; set; }
        /// <summary>
        /// (PhaseShift) Indicates if the chart uses SysEx events for open notes.
        /// </summary>
        public bool? SysExOpenBass { get; set; }
        /// <summary>
        /// (PhaseShift) Indicates if the chart uses SysEx events for Pro Guitar/Bass slide directions.
        /// </summary>
        public bool? SysexProSlide { get; set; }
        #endregion
    }
}