using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.SystemExtensions.Linq;

namespace ChartTools.IO.Chart.Parsing
{
    internal class DrumsTrackParser : TrackParser<DrumsChord>
    {
        public DrumsTrackParser(Difficulty difficulty, ReadingSession session, string header) : base(difficulty, session, header) { }

        public override void ApplyToSong(Song song)
        {
            song.Instruments.Drums ??= new();
            ApplyToInstrument(song.Instruments.Drums);
        }

        protected override void HandleNoteEntry(DrumsChord chord, NoteData data)
        {
            switch (data.NoteIndex)
            {
                // Note
                case < 5:
                    AddNote(new DrumsNote((DrumsLane)data.NoteIndex) { Length = data.SustainLength });
                    break;
                // Double kick
                case 32:
                    AddNote(new DrumsNote(DrumsLane.DoubleKick));
                    break;
                // Cymbal
                case > 65 and < 69:
                    // NoteIndex of the note to set as cymbal
                    byte seekedIndex = (byte)(data.NoteIndex - 64);

                    if (chord.Notes.TryGetFirst(n => n.NoteIndex == seekedIndex, out DrumsNote note))
                    {
                        if (session.DuplicateTrackObjectProcedure(chord.Position, "drums note cymbal marker", () => note.IsCymbal))
                            note.IsCymbal = true;
                    }
                    else
                        AddNote(new DrumsNote((DrumsLane)seekedIndex) { IsCymbal = true, Length = data.SustainLength });
                    break;
                case 109:
                    AddModifier(DrumsChordModifier.Flam);
                    break;
            }

            void AddNote(DrumsNote note) => HandleAddNote(note, () => chord.Notes.Add(note));
            void AddModifier(DrumsChordModifier modifier) => HandleAddModifier(chord.Modifier, modifier, () => chord.Modifier |= modifier);
        }

        protected override DrumsChord CreateChord(uint position) => new(position);
    }
}
