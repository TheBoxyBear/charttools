using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.SystemExtensions.Linq;

using System;

namespace ChartTools.IO.Chart.Parsers
{
    internal class DrumsTrackParser : TrackParser<DrumsChord>
    {
        public DrumsTrackParser(Difficulty difficulty, ReadingSession session) : base(difficulty, session) { }

        public override void ApplyToSong(Song song) => ApplyToInstrument(song.Instruments.Drums ??= new());

        protected override void HandleNote(Track<DrumsChord> track, ref DrumsChord chord, uint position, NoteData data, ref bool newChord, out Enum initialModifier)
        {
            // Find the parent chord or create it
            if (chord is null)
                chord = new(position);
            else if (position != chord.Position)
                chord = track.Chords.FirstOrDefault(c => c.Position == position, new(position), out newChord)!;
            else
                newChord = false;

            initialModifier = chord!.Modifier;

            switch (data.NoteIndex)
            {
                // Note
                case < 5:
                    chord!.Notes.Add(new DrumsNote((DrumsLane)data.NoteIndex) { Length = data.SustainLength });
                    break;
                // Double kick
                case 32:
                    chord!.Notes.Add(new DrumsNote(DrumsLane.DoubleKick));
                    break;
                // Cymbal
                case > 65 and < 69:
                    DrumsNote? note = null;
                    // NoteIndex of the note to set as cymbal
                    byte seekedIndex = (byte)(data.NoteIndex - 64);

                    // Find matching note
                    note = chord!.Notes.FirstOrDefault(n => n.NoteIndex == seekedIndex, null, out bool returnedDefault);

                    if (returnedDefault)
                    {
                        chord.Notes.Add(new DrumsNote((DrumsLane)seekedIndex) { IsCymbal = true, Length = data.SustainLength });
                        returnedDefault = false;
                    }
                    else
                        note!.IsCymbal = true;
                    break;
                case 109:
                    chord!.Modifier |= DrumsChordModifier.Flam;
                    break;
            }
        }
    }
}
