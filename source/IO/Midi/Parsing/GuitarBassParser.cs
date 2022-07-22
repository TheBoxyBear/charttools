using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

using Melanchall.DryWetMidi.Core;

using System;
using System.Collections.Generic;

namespace ChartTools.IO.Midi.Parsing
{
    internal class GuitarBassParser : StandardInstrumentParser
    {
        private GuitarBassFormat format;
        private readonly List<MidiMappingResult> mappings = new();

        public GuitarBassParser(StandardInstrumentIdentity instrument, ReadingSession session) : base(instrument, new GuitarBassMapper(), session)
        {
            if (instrument is not StandardInstrumentIdentity.LeadGuitar or StandardInstrumentIdentity.Bass)
                throw new ArgumentException("Instrument must be lead guitar or bass.", nameof(instrument));
        }

        protected override bool CustomHandle(NoteEvent note)
        {
            var newFormat = GuitarBassFormat.NAUnknown;
            var newMappings = mapper.MapNoteEvent(globalPosition, note);

            foreach (var mapping in mapper.MapNoteEvent(globalPosition, note))
            {
                if (mapping.Type is MappingType.Animation || (note.NoteNumber == 116))
                    newFormat = GuitarBassFormat.RockBand;
                else if (mapping.Type is MappingType.Special)
                {
                    if (mapping.Index is (byte)SpecialPhraseType.Trill or (byte)SpecialPhraseType.Tremolo)
                        newFormat = GuitarBassFormat.RockBand;
                    else if (mapping.Difficulty is not Difficulty.Expert)
                        newFormat = GuitarBassFormat.GuitarHero2;
                }

                if (format is GuitarBassFormat.NAUnknown)
                    format = newFormat;
                else if (format != newFormat)
                    format = session.UncertainGuitarBassFormatProcedure(Instrument);

                mappings.AddRange(newMappings);
            }

            return true;
        }

        protected override void FinaliseParse()
        {
            if (format is GuitarBassFormat.NAUnknown)
                format = session.UncertainGuitarBassFormatProcedure(Instrument);

            foreach (var mapping in mappings)
            {
                switch (mapping.Type)
                {
                    case MappingType.Note:
                        BaseHandle(mapping);
                        break;
                    case MappingType.Modifier:
                        if (format.HasFlag(GuitarBassFormat.RockBand) && mapping.Difficulty == Difficulty.Expert)
                            BaseHandle(mapping);
                        break;
                    case MappingType.Special when format.HasFlag(GuitarBassFormat.RockBand):
                        if (mapping.Index is (byte)SpecialPhraseType.Trill or (byte)SpecialPhraseType.Tremolo || mapping.Difficulty is null)
                            BaseHandle(mapping);
                        break;
                    case MappingType.Special when format.HasFlag(GuitarBassFormat.GuitarHero2):
                        if (mapping.Index is not (byte)SpecialPhraseType.Trill or (byte)SpecialPhraseType.Tremolo)
                            BaseHandle(mapping);
                        break;
                }
            }
        }
    }
}
