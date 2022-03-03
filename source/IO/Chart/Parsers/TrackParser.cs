using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.SystemExtensions.Linq;
using ChartTools.Tools.Optimizing;

using System;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Parsers
{
    internal abstract class TrackParser<TChord> : FileParser<string> where TChord : Chord
    {
        public Difficulty Difficulty { get; }

        private TChord? chord;
        private bool newChord = true;
        private readonly HashSet<byte> ignoredNotes = new();

        public override Track<TChord> Result => GetResult(result);
        private readonly Track<TChord> result = new();

        public TrackParser(Difficulty difficulty, ReadingSession session) : base(session) => Difficulty = difficulty;

        protected override void HandleItem(string line)
        {
            TrackObjectEntry entry;

            try { entry = new(line); }
            catch (Exception e) { throw ChartExceptions.Line(line, e); }

            switch (entry.Type)
            {
                // Local event
                case "E":
                    string[] split = ChartFile.GetDataSplit(entry.Data.Trim('"'));
                    result.LocalEvents!.Add(new(entry.Position, split.Length > 0 ? split[0] : string.Empty));
                    break;
                // Note or chord modifier
                case "N":
                    NoteData data;
                    try
                    {
                        data = new(entry.Data);

                        HandleNote(result, ref chord!, entry.Position, data, ref newChord, out Enum initialModifier);

                        if (newChord)
                        {
                            result.Chords.Add(chord!);
                            ignoredNotes.Clear();
                        }
                    }
                    catch (Exception e) { throw ChartExceptions.Line(line, e); }

                    break;
                // Star power
                case "S":
                    try
                    {
                        split = ChartFile.GetDataSplit(entry.Data);

                        if (!byte.TryParse(split[0], out byte typeCode))
                            throw new FormatException($"Cannot parse type code \"{split[0]}\" to byte.");
                        if (!uint.TryParse(split[1], out uint length))
                            throw new FormatException($"Cannot parse length \"{split[1]}\" to uint.");

                        result.StarPower.Add(new(entry.Position, typeCode, length));
                    }
                    catch (Exception e) { throw ChartExceptions.Line(line, e); }
                    break;
            }

            if (session!.Configuration.SoloNoStarPowerPolicy == SoloNoStarPowerPolicy.Convert)
                result.StarPower.AddRange(result.SoloToStarPower(true));
        }

        protected abstract void HandleNote(Track<TChord> track, ref TChord chord, uint position, NoteData data, ref bool newChord, out Enum initialModifier);

        protected override void FinaliseParse()
        {
            base.FinaliseParse();

            ApplyOverlappingSpecialPhrasePolicy(result.StarPower, session!.Configuration.OverlappingStarPowerPolicy);
        }

        protected void ApplyResultToInstrument(Instrument<TChord> instrument) => instrument.SetTrack(Result, Difficulty);

        private static void ApplyOverlappingSpecialPhrasePolicy(IEnumerable<SpecicalPhrase> specialPhrases, OverlappingSpecialPhrasePolicy policy)
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
