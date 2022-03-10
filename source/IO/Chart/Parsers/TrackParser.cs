using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.SystemExtensions.Linq;
using ChartTools.Tools.Optimizing;

using System;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Parsers
{
    internal abstract class TrackParser<TChord> : ChartParser, IInstrumentAppliable<TChord> where TChord : Chord
    {
        public Difficulty Difficulty { get; }

        public override Track<TChord> Result => GetResult(result);
        private readonly Track<TChord> result;

        private TChord? chord;
        private bool newChord = true;
        private readonly HashSet<byte> ignoredNotes = new();

        public TrackParser(Difficulty difficulty, ReadingSession session) : base(session)
        {
            Difficulty = difficulty;
            result = new() { Difficulty = difficulty };
        }

        protected override void HandleItem(string line)
        {
            TrackObjectEntry entry = new(line);

            switch (entry.Type)
            {
                // Local event
                case "E":
                    result.LocalEvents.Add(new(entry.Position, entry.Data));
                    break;
                // Note or chord modifier
                case "N":
                    NoteData data = new(entry.Data);

                    HandleNote(result, ref chord!, entry.Position, data, ref newChord, out Enum initialModifier);

                    if (newChord)
                    {
                        result.Chords.Add(chord!);
                        ignoredNotes.Clear();
                    }

                    break;
                // Star power
                case "S":
                    var split = ChartFile.GetDataSplit(entry.Data);

                    var typeCode = ValueParser.ParseByte(split[0], "type code");
                    var length = ValueParser.ParseUint(split[1], "length");

                    result.StarPower.Add(new(entry.Position, typeCode, length));
                    break;
            }

            if (session!.Configuration.SoloNoStarPowerPolicy == SoloNoStarPowerPolicy.Convert)
                result.StarPower.AddRange(result.SoloToStarPower(true));
        }

        protected abstract void HandleNote(Track<TChord> track, ref TChord chord, uint position, NoteData data, ref bool newChord, out Enum initialModifier);

        protected override void FinaliseParse()
        {
            ApplyOverlappingSpecialPhrasePolicy(result.StarPower, session!.Configuration.OverlappingStarPowerPolicy);
            base.FinaliseParse();
        }

        public void ApplyToInstrument(Instrument<TChord> instrument) => instrument.SetTrack(Result);

        private static void ApplyOverlappingSpecialPhrasePolicy(IEnumerable<SpecialPhrase> specialPhrases, OverlappingSpecialPhrasePolicy policy)
        {
            switch (policy)
            {
                case OverlappingSpecialPhrasePolicy.Cut:
                    specialPhrases.CutLengths();
                    break;
                case OverlappingSpecialPhrasePolicy.ThrowException:
                    foreach ((var previous, var current) in specialPhrases.RelativeLoop())
                        if (Optimizer.LengthNeedsCut(previous!, current!))
                            throw new Exception($"Overlapping star power phrases at position {current!.Position}. Consider using {nameof(OverlappingSpecialPhrasePolicy.Cut)} to avoid this error.");
                    break;
            }
        }
    }
}
