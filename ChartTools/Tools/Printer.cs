namespace ChartTools.Tools;

internal static class Printer
{
	private readonly struct ConsoleContent(string content, ConsoleColor color)
	{
		public string Content { get; } = content;
		public ConsoleColor Color { get; } = color;
	}

	public static void PrintTrack(Track<StandardChord> track)
	{
		List<List<ConsoleContent>> content = [];
		uint[] sustainEnds = new uint[6];
		ConsoleColor[] laneColors =
		[
			ConsoleColor.Green,
			ConsoleColor.Red,
			ConsoleColor.Yellow,
			ConsoleColor.Blue,
			ConsoleColor.DarkYellow
		];

		foreach (StandardChord chord in track.Chords.Where(c => c.Notes.Count > 0).OrderBy(t => t.Position))
		{
			LaneNote<StandardLane>? open = chord.Notes[StandardLane.Open];
			List<ConsoleContent> lineContent = [];

			if (open is not null)
			{
				lineContent.Add(new("-----", ConsoleColor.Magenta));

				SetSustainEnd(open.Value);

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
						LaneNote<StandardLane>? note = chord.Notes[(StandardLane)i];
						string text;

						if (note is null)
							text = sustainEnds[i] >= chord.Position ? "|" : " ";
						else
						{
							text = "O";
							SetSustainEnd(note.Value);
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
		foreach (IEnumerable<ConsoleContent> line in content.Reverse())
		{
			Console.WriteLine();

			foreach (ConsoleContent ct in line)
			{
				Console.ForegroundColor = ct.Color;
				Console.Write(ct.Content);
			}
		}

		Console.ResetColor();
	}
}
