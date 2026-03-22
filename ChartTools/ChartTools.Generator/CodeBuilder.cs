using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.Generator;

internal class CodeBuilder(StringBuilder? builder = default)
{
	public CodeBuilder(CodeBuilder? builder = default)
		: this(new StringBuilder(builder?.ToString() ?? null))
	{
		m_contextStack = builder?.m_contextStack ?? [];
		m_indent = builder?.m_indent ?? string.Empty;
	}

	public CodeBuilder(string value)
		: this(new StringBuilder(value)) { }

	public CodeBuilder(int indent, StringBuilder? builder = default)
		: this(builder)
		=> m_indent = new('\t', indent);

	public StringBuilder StringBuilder { get; } = builder ?? new();

	private readonly Stack<char> m_contextStack = [];
	private string m_indent = string.Empty;

	private static readonly Dictionary<char, char> m_contextClosers = new()
	{
		{ '(', ')' },
		{ '{', '}' },
		{ '[', ']' },
		{ '<', '>' },
		{ '"', '"' }
	};

	public CodeBuilder StartContext(char openChar, bool newLine = true)
	{
		StringBuilder.Append(m_indent + openChar);

		if (newLine)
			StringBuilder.AppendLine();

		m_contextStack.Push(openChar);
		m_indent += '\t';

		return this;
	}

	public CodeBuilder EndContext(bool newLine = true)
	{
		if (m_contextStack.Count == 0)
			throw new InvalidOperationException("No context to end.");

		char openChar = m_contextStack.Pop();

		if (!m_contextClosers.TryGetValue(openChar, out char closeChar))
			throw new InvalidOperationException($"No closing character defined for '{openChar}'.");

		m_indent = m_indent[..^1];

		StringBuilder.Append(m_indent + closeChar);

		if (newLine)
			StringBuilder.AppendLine();

		return this;
	}

	public CodeBuilder EndAllContexts(bool newLine = true)
	{
		while (m_contextStack.Count > 0)
			EndContext(newLine);

		return this;
	}

	public CodeBuilder AppendLine(in ReadOnlySpan<char> line)
	{
		StringBuilder.AppendLine(m_indent + line.ToString());
		return this;
	}

	public CodeBuilder AppendLine()
	{
		StringBuilder.AppendLine(m_indent);
		return this;
	}

	public CodeBuilder AppendLines(ReadOnlySpan<char> lines)
	{
		int breakIndex;

		while ((breakIndex = lines.IndexOf('\n')) != -1)
		{
			ReadOnlySpan<char> line = lines[..breakIndex];

			Append(line);
			lines = lines[(breakIndex + 1)..];
		}

		if (lines.Length > 0)
			AppendLine(lines);

		return this;
	}

	public CodeBuilder Append(in ReadOnlySpan<char> text)
	{
		StringBuilder.Append(m_indent + text.ToString());
		return this;
	}

	public CodeBuilder AppendInstruction(in ReadOnlySpan<char> instruction)
	{
		StringBuilder.AppendLine(m_indent + instruction.ToString() + ';');
		return this;
	}

	public override string ToString()
	{
		EndAllContexts();
		return StringBuilder.ToString();
	}

	public static implicit operator StringBuilder(CodeBuilder builder)
		=> builder.StringBuilder;
}
