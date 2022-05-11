using ChartTools.Internal;
using ChartTools.IO.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Midi.Parsers
{
    internal class GHGemsParser : StandardInstrumentParser
    {
        private readonly Dictionary<Difficulty, Dictionary<StandardLane, StandardChord?>> openedNoteSources = new(from difficulty in EnumCache<Difficulty>.Values
                                                                                                                  let pairs = from lane in EnumCache<StandardLane>.Values
                                                                                                                              select new KeyValuePair<StandardLane, StandardChord?>(lane, null)
                                                                                                                  select new KeyValuePair<Difficulty, Dictionary<StandardLane, StandardChord?>>(difficulty, new(pairs)));
        private readonly Dictionary<Difficulty, StandardChord?> previousChords = new(from difficulty in EnumCache<Difficulty>.Values select new KeyValuePair<Difficulty, StandardChord?>(difficulty, null));
        private readonly Dictionary<Difficulty, uint?> openedStarPowerPositions = new(EnumCache<Difficulty>.Values.Select(difficulty => new KeyValuePair<Difficulty, uint?>(difficulty, null)));
        private uint globalPosition;
        public GHGemsParser(ReadingSession session) : base(StandardInstrumentIdentity.LeadGuitar, session) { }

        protected override void HandleItem(MidiEvent item)
        {
            if (item is not NoteEvent e)
                return;

            (var track, var adjusted) = MapNoteEvent(e);

            if (track is null)
                return;

            globalPosition += (uint)item.DeltaTime;

            if (adjusted < 5) // Note
            {
                var lane = (StandardLane)adjusted;
                var openedSource = openedNoteSources[track.Difficulty][lane];

                switch (e)
                {
                    case NoteOnEvent:
                        if (openedSource is not null)
                            session.HandleUnclosed(openedSource.Position, () => openedSource.Notes[lane]!.Length = globalPosition - openedSource.Position);

                        var chord = openedNoteSources[track.Difficulty][lane] = GetOrCreateChord(globalPosition);
                        chord.Notes.Add(lane);
                        break;
                    case NoteOffEvent:
                        if (openedSource is null)
                            session.HandleUnopened(globalPosition, () => GetOrCreateChord(globalPosition).Notes.Add(lane));
                        else
                        {
                            openedSource.Notes[lane]!.Length = globalPosition - openedSource.Position;
                            openedNoteSources[track.Difficulty][lane] = null;
                        }
                        break;
                }

                StandardChord GetOrCreateChord(uint newChordPosition)
                {
                    var chord = previousChords[track.Difficulty];

                    if (chord is null)
                        track.Chords.Add(chord = previousChords[track.Difficulty] = new(newChordPosition));

                    return chord;
                }
            }
            else if (adjusted < 11) // Special
            {
                var openedPosition = openedStarPowerPositions[track.Difficulty];

                switch (e)
                {
                    case NoteOnEvent:
                        if (openedPosition is not null)
                            session.HandleUnclosed(openedPosition.Value, CloseStarPower);

                        openedStarPowerPositions[track.Difficulty] = globalPosition;
                        break;
                    case NoteOffEvent:
                        if (openedPosition is null)
                            session.HandleUnopened(globalPosition, () => track.SpecialPhrases.Add(new(globalPosition, SpecialPhraseType.StarPowerGain)));
                        else
                            CloseStarPower();
                        break;
                }

                void CloseStarPower() => track.SpecialPhrases.Add(new(globalPosition, SpecialPhraseType.StarPowerGain, globalPosition - openedPosition!.Value));
            }
        }

        protected override (Track<StandardChord>? track, int adjustedNoteNumber) MapNoteEvent(NoteEvent e)
        {
            var intNumber = (int)e.NoteNumber;

            return intNumber switch
            {
                > 59 and < 71 => (GetOrCreateTrack(Difficulty.Easy), intNumber - 60),
                > 71 and < 83 => (GetOrCreateTrack(Difficulty.Medium), intNumber - 72),
                > 83 and < 95 => (GetOrCreateTrack(Difficulty.Hard), intNumber - 84),
                > 95 and < 107 => (GetOrCreateTrack(Difficulty.Expert), intNumber - 96),
                _ => (null, 0)
            };
        }
    }
}
