using ChartTools.Extensions.Linq;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Chart.Parsing;

internal class DrumsTrackParser(Difficulty difficulty, ChartReadingSession session, string header)
	: TrackParser<DrumsChord>(difficulty, session, header)
{
	public override void ApplyToSong(Song song)
	{
		song.Instruments.Drums ??= new();
		ApplyToInstrument(song.Instruments.Drums);
	}

	protected override void HandleNoteEntry(DrumsChord chord, in NoteData data)
	{
		switch (data.Index)
		{
			// Note
			case < 5:
				AddNote(new DrumsNote((DrumsLane)data.Index) { Sustain = data.SustainLength });
				break;
			// Double kick
			case 32:
				AddNote(new DrumsNote(DrumsLane.DoubleKick));
				break;
			// Cymbal
			case > 65 and < 69:
				// NoteIndex of the note to set as cymbal
				byte seekedIndex = (byte)(data.Index - 64);

				if (chord.Notes.TryGetFirst(n => n.Index == seekedIndex, out DrumsNote note))
				{
					if (Session.HandleDuplicate(chord.Position, "drums note cymbal marker", () => note.IsCymbal))
						note.IsCymbal = true;
				}
				else
					AddNote(new((DrumsLane)seekedIndex) { IsCymbal = true, Sustain = data.SustainLength });
				break;
			case 109:
				AddModifier(DrumsChordModifiers.Flam);
				break;
		}

		void AddNote(DrumsNote note)
		{
			if (CanAddNote(note.Index))
				chord.Notes.Add(note);
		}

		void AddModifier(DrumsChordModifiers modifier)
		{
			if (CanAddModifier(chord.Modifiers, modifier))
				chord.Modifiers |= modifier;
		}
	}
}
