using ChartTools.Internal;
using ChartTools.IO.Configuration.Sessions;

using Melanchall.DryWetMidi.Core;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Midi.Parsers
{
    internal class GHGemsParser : StandardInstrumentParser
    {
        private readonly Dictionary<Difficulty, Dictionary<StandardLane, StandardChord?>>
            openedNoteSources = new(from difficulty in EnumCache<Difficulty>.Values
                                    let pairs = from lane in EnumCache<StandardLane>.Values
                                                select new KeyValuePair<StandardLane, StandardChord?>(lane, null)
                                    select new KeyValuePair<Difficulty, Dictionary<StandardLane, StandardChord?>>(difficulty, new(pairs)));

        private readonly Dictionary<Difficulty, StandardChord?> previousChords = new(from difficulty in EnumCache<Difficulty>.Values select new KeyValuePair<Difficulty, StandardChord?>(difficulty, null));

        private readonly Dictionary<Difficulty, Dictionary<SpecialPhraseType, uint?>>
            openedSpecialPositions = new(from difficulty in EnumCache<Difficulty>.Values
                                         let pairs = from type in EnumCache<SpecialPhraseType>.Values
                                                     select new KeyValuePair<SpecialPhraseType, uint?>(type, null)
                                         select new KeyValuePair<Difficulty, Dictionary<SpecialPhraseType, uint?>>(difficulty, new(pairs)));

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

            switch (adjusted)
            {
                case < 6: // Note
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

                        if (chord is null || chord.Position != newChordPosition)
                            track.Chords.Add(chord = previousChords[track.Difficulty] = new(newChordPosition));

                        return chord;
                    }
                    break;
                case < 12: // Special
                    var type = adjusted switch
                    {
                        8 => SpecialPhraseType.StarPowerGain,
                        10 => SpecialPhraseType.Player1FaceOff,
                        11 => SpecialPhraseType.Player2FaceOff,
                        _ => throw new System.Exception($"Invalid note number {e.NoteNumber} at position {globalPosition}") // TODO Better exception
                    };
                    var openedPosition = openedSpecialPositions[track.Difficulty][type];

                    switch (e)
                    {
                        case NoteOnEvent:
                            if (openedPosition is not null)
                                session.HandleUnclosed(openedPosition.Value, CloseSpecial);

                            openedSpecialPositions[track.Difficulty][type] = globalPosition;
                            break;
                        case NoteOffEvent:
                            if (openedPosition is null)
                                session.HandleUnopened(globalPosition, () => track.SpecialPhrases.Add(new(globalPosition, type)));
                            else
                            {
                                CloseSpecial();
                                openedSpecialPositions[track.Difficulty][type] = null;
                            }
                            break;
                    }

                    void CloseSpecial() => track.SpecialPhrases.Add(new(globalPosition, type, globalPosition - openedPosition!.Value));
                    break;
            }
        }

        protected override (Track<StandardChord>? track, int adjustedNoteNumber) MapNoteEvent(NoteEvent e)
        {
            var intNumber = (int)e.NoteNumber;

            return intNumber switch
            {
                > 59 and < 71 => (GetOrCreateTrack(Difficulty.Easy), intNumber - 59),
                > 71 and < 83 => (GetOrCreateTrack(Difficulty.Medium), intNumber - 71),
                > 83 and < 95 => (GetOrCreateTrack(Difficulty.Hard), intNumber - 83),
                > 95 and < 107 => (GetOrCreateTrack(Difficulty.Expert), intNumber - 95),
                _ => (null, 0)
            };
        }
    }
}
