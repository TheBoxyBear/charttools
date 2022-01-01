using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Chart.Sessions;
using ChartTools.SystemExtensions.Linq;
using ChartTools.Tools.Optimizing;

using System;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Parsers
{
    internal abstract class TrackParser<TChord> : ChartPartParser where TChord : Chord
    {
        public Difficulty Difficulty { get; }

        private Track<TChord>? preResult, result;
        private TChord? chord;
        private bool newChord = true;
        private HashSet<byte> ignoredNotes = new();

        public override Track<TChord>? Result => result;

        public TrackParser(Difficulty difficulty) => Difficulty = difficulty;

        protected override void HandleLine(string line)
        {
            TrackObjectEntry entry;

            try { entry = new(line); }
            catch (Exception e) { throw ChartParser.GetLineException(line, e); }

            switch (entry.Type)
            {
                // Local event
                case "E":
                    string[] split = ChartParser.GetDataSplit(entry.Data.Trim('"'));
                    preResult!.LocalEvents!.Add(new(entry.Position, split.Length > 0 ? split[0] : string.Empty));
                    break;
                // Note or chord modifier
                case "N":
                    NoteData data;
                    try
                    {
                        data = new(entry.Data);
                        HandleNote(preResult!, ref chord!, entry.Position, data, ref newChord, out Enum initialModifier);

                        if (newChord)
                        {
                            preResult!.Chords.Add(chord!);
                            ignoredNotes.Clear();
                        }
                    }
                    catch (Exception e) { throw ChartParser.GetLineException(line, e); }

                    break;
                // Star power
                case "S":
                    try
                    {
                        split = ChartParser.GetDataSplit(entry.Data);

                        if (!uint.TryParse(split[1], out uint length))
                            throw new FormatException($"Cannot parse length \"{split[0]}\" to uint.");

                        preResult!.StarPower.Add(new(entry.Position, length));
                    }
                    catch (Exception e) { throw ChartParser.GetLineException(line, e); }
                    break;
            }

            if (session!.Configuration.SoloNoStarPowerPolicy == SoloNoStarPowerPolicy.Convert)
                preResult!.StarPower.AddRange(preResult!.SoloToStarPower(true));
        }

        protected abstract void HandleNote(Track<TChord> track, ref TChord chord, uint position, NoteData data, ref bool newChord, out Enum initialModifier);

        protected override void PrepareParse()
        {
            preResult = new();
            ignoredNotes = new();
        }
        protected override void FinaliseParse()
        {
            ApplyOverlappingStarPowerPolicy(preResult!.StarPower, session!.Configuration.OverlappingStarPowerPolicy);
            result = preResult;
        }

        public void ApplyResultToInstrument(Instrument<TChord> instrument) => instrument.SetTrack(result!, Difficulty);

        private static void ApplyOverlappingStarPowerPolicy(IEnumerable<StarPowerPhrase> starPower, OverlappingStarPowerPolicy policy)
        {
            switch (policy)
            {
                case OverlappingStarPowerPolicy.Cut:
                    starPower.CutLengths();
                    break;
                case OverlappingStarPowerPolicy.ThrowException:
                    foreach ((var previous, var current) in starPower.RelativeLoop())
                        if (Optimizer.LengthNeedsCut(previous!, current!))
                            throw new Exception($"Overlapping star power phrases at position {current!.Position}. Consider using {nameof(OverlappingStarPowerPolicy.Cut)} to avoid this error.");
                    break;
            }
        }
    }
}
