﻿using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Parsing;

internal abstract class InstrumentParser<TChord> : MidiParser where TChord : IChord, new()
{
    protected readonly Track<TChord>[] tracks = new Track<TChord>[4];

    public override Instrument<TChord> Result => GetResult(GetInstrument());

    protected InstrumentParser(ReadingSession session) : base(session) => ArgumentNullException.ThrowIfNull(session, nameof(session));

    protected abstract Instrument<TChord> GetInstrument();
    protected abstract IEnumerable<NoteEventMapping> MapNoteEvent(uint position, NoteEvent e);
    protected override void FinaliseParse()
    {
        foreach (var track in tracks)
            GetInstrument().SetTrack(track);

        base.FinaliseParse();
    }

    protected uint GetSustain(uint start, uint end)
    {
        var length = end - start;
        return length < session.Formatting?.SustainCutoff ? 0 : length;
    }
}