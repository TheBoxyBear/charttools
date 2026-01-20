using ChartTools.Extensions.Collections;
using ChartTools.IO.Parsing;
using ChartTools.IO.Sources;

namespace ChartTools.IO;

internal abstract class TextFileReader(ReadingDataSource source) : FileReader<ReadOnlyMemory<char>, TextParser>(source)
{
	public virtual bool DefinedSectionEnd { get; } = false;

	protected bool _disposeReader = false;

	protected override void ReadBase(bool async, in CancellationToken cancellationToken)
	{
		string contentStr;

		using (Source.Stream)
		{
			using StreamReader reader = new(Source.Stream, leaveOpen: true);
			contentStr = reader.ReadToEnd();
		}

		ReadOnlyMemory<char> content = contentStr.AsMemory();
		ReadOnlyMemory<char> line    = string.Empty.AsMemory();

		ParserContentGroup? currentGroup = null;

		while (ReadLine(ref content, ref line))
		{
			// Find section
			while (!line.Span.StartsWith('['))
				if (!ReadLine(ref content, ref line))
					return;

			if (async && cancellationToken.IsCancellationRequested)
			{
				Dispose();
				return;
			}

			ReadOnlyMemory<char> header = line;
			TextParser? parser = GetParser(header);

			if (parser is not null)
			{
				DelayedEnumerableSource<ReadOnlyMemory<char>> source = new();

				parserGroups.Add(currentGroup = new(parser, source));

				if (async)
				{
					if (cancellationToken.IsCancellationRequested)
					{
						Dispose();
						return;
					}

					parseTasks.Add(parser.StartAsyncParse(source.Enumerable));
				}
			}

			// Move to the start of the entries
			do
				if (!AdvanceSection())
				{
					FinishSection(cancellationToken);
					return;
				}
			while (!IsSectionStart(line.Span));

			AdvanceSection();

			// Read until end
			while (!IsSectionEnd(line.Span))
			{
				currentGroup?.Source.Add(line);

				if (!AdvanceSection())
				{
					FinishSection(cancellationToken);
					return;
				}
			}

			FinishSection(cancellationToken);

			void FinishSection(in CancellationToken cancellationToken)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					Dispose();
					return;
				}

				currentGroup?.Source.EndAwait();
			}

			bool AdvanceSection() => ReadLine(ref content, ref line) || (DefinedSectionEnd ? throw SectionException.EarlyEnd(header.ToString()) : false);
		}

		bool ReadLine(ref ReadOnlyMemory<char> content, ref ReadOnlyMemory<char> line)
		{
			while (true)
			{
				int newLineIndex = content.Span.IndexOf('\n');

				if (newLineIndex == -1)
				{
					line = content.Trim();
					return false;
				}

				line    = content[..newLineIndex].Trim();
				content = content[(newLineIndex + 1)..];

				if (line.Length > 0)
					break;
			}

			return true;
		}
	}

	protected abstract bool IsSectionStart(in ReadOnlySpan<char> line);

	protected virtual bool IsSectionEnd(in ReadOnlySpan<char> line) => false;
}
