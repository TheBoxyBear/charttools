using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools
{
    public record StandardInstrument : Instrument<StandardChord>
    {
        public new StandardInstrumentIdentity InstrumentIdentity { get; init; }


        /// <summary>
        /// Format of lead guitar and bass. Not applicable to other instruments.
        /// </summary>
        public MidiInstrumentOrigin MidiOrigin
        {
            get => midiOrigin;
            set
            {
                Validator.ValidateEnum(value);

                if (InstrumentIdentity is StandardInstrumentIdentity.LeadGuitar)
                {
                    if (value.HasFlag(MidiInstrumentOrigin.GuitarHero1))
                        Error("Guitar Hero 1");
                }
                else if (value.HasFlag(MidiInstrumentOrigin.GuitarHero2) && InstrumentIdentity is not StandardInstrumentIdentity.RhythmGuitar or StandardInstrumentIdentity.CoopGuitar or StandardInstrumentIdentity.Bass)
                    Error("Guitar Hero 2");

                void Error(string origin) => throw new ArgumentException($"{InstrumentIdentity} is not supported by {origin}.", nameof(value));

                midiOrigin = value;
            }
        }
        private MidiInstrumentOrigin midiOrigin;

        public StandardInstrument() { }
        public StandardInstrument(StandardInstrumentIdentity identity)
        {
            Validator.ValidateEnum(identity);
            InstrumentIdentity = identity;
        }

        protected override InstrumentIdentity GetIdentity() => (InstrumentIdentity)InstrumentIdentity;

        #region File reading
        /// <summary>
        /// Reads a standard instrument from a file.
        /// </summary>
        public static StandardInstrument? FromFile(string path, StandardInstrumentIdentity instrument, ReadingConfiguration? config = default, FormattingRules? formatting = default)
        {
            Validator.ValidateEnum(instrument);
            return ExtensionHandler.Read(path, (".chart", path => ChartFile.ReadInstrument(path, instrument, config, formatting)));
        }
        /// <summary>
        /// Reads a standard instrument from a file asynchronously using multitasking.
        /// </summary>
        public static async Task<StandardInstrument?> FromFileAsync(string path, StandardInstrumentIdentity instrument, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.ReadAsync(path, (".chart", path => ChartFile.ReadInstrumentAsync(path, instrument, config, formatting, cancellationToken)));

        public static DirectoryResult<StandardInstrument?> FromDirectory(string directory, StandardInstrumentIdentity instrument, ReadingConfiguration? config = default) => DirectoryHandler.FromDirectory(directory, (path, formatting) => FromFile(path, instrument, config, formatting));
        public static Task<DirectoryResult<StandardInstrument?>> FromDirectoryAsync(string directory, StandardInstrumentIdentity instrument, ReadingConfiguration? config = default, CancellationToken cancellationToken = default) => DirectoryHandler.FromDirectoryAsync(directory, async (path, formatting) => await FromFileAsync(path, instrument, config, formatting, cancellationToken), cancellationToken);
        #endregion
    }
}
