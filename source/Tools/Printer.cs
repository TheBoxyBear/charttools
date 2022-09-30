using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Tools
{
    public static class Printer
    {
        private readonly struct ConsoleContent
        {
            public string Content { get; }
            public ConsoleColor Color { get; }

            public ConsoleContent(string content, ConsoleColor color)
            {
                Content = content;
                Color = color;
            }
        }

        public static void PrintTrack(Track<StandardChord> track)
        {
            var content = new List<List<ConsoleContent>>();
            uint[] sustainEnds = new uint[6];
            ConsoleColor[] laneColors = new ConsoleColor[]
            {
                ConsoleColor.Green,
                ConsoleColor.Red,
                ConsoleColor.Yellow,
                ConsoleColor.Blue,
                ConsoleColor.DarkYellow
            };

            foreach (var chord in track.Chords.Where(c => c.Notes.Count > 0).OrderBy(t => t.Position))
            {
                var open = chord.Notes[StandardLane.Open];
                var lineContent = new List<ConsoleContent>();

                if (open is not null)
                {
                    lineContent.Add(new("-----", ConsoleColor.Magenta));

                    SetSustainEnd(open);

                    for (int i = 1; i < sustainEnds.Length; i++)
                        sustainEnds[i] = chord.Position;
                }
                else
                {
                    if (chord.Notes.Count == 0)
                        lineContent.Add(new(sustainEnds[0] >= chord.Position ? "  |  " : "     ", ConsoleColor.Magenta));
                    else
                        for (int i = 1; i < 6; i++)
                        {
                            var note = chord.Notes[(StandardLane)i];
                            string text;

                            if (note is null)
                                text = sustainEnds[i] >= chord.Position ? "|" : " ";
                            else
                            {
                                text = "O";
                                SetSustainEnd(note);
                            }

                            lineContent.Add(new(text, laneColors[i - 1]));
                        }
                }

                content.Add(lineContent);

                void SetSustainEnd(LaneNote<StandardLane> note) => sustainEnds[(int)note.Lane] = chord.Position + note.Sustain;
            }

            PrintLines(content);
        }

        private static void PrintLines(IEnumerable<IEnumerable<ConsoleContent>> content)
        {
            foreach (var line in content.Reverse())
            {
                Console.WriteLine();

                foreach (var ct in line)
                {
                    Console.ForegroundColor = ct.Color;
                    Console.Write(ct.Content);
                }
            }

            Console.ResetColor();
        }
    }
}
