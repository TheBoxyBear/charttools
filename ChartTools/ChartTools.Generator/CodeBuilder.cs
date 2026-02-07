using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.Generator;

internal class CodeBuilder(StringBuilder? builder = default)
{
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
		{ '"', '"' }
	};

	public CodeBuilder StartContext(char openChar, bool newLine = true)
	{
		if (newLine)
			StringBuilder.AppendLine(m_indent + openChar);
		else
			StringBuilder.Append(m_indent + openChar);

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

		m_indent = m_indent.Substring(0, m_indent.Length - 2);

		if (newLine)
			StringBuilder.AppendLine(m_indent + closeChar);
		else
			StringBuilder.Append(m_indent + closeChar);

		return this;
	}

	public CodeBuilder AppendLine(string line)
	{
		StringBuilder.AppendLine(m_indent + line);
		return this;
	}

	public CodeBuilder AppendLine()
	{
		StringBuilder.AppendLine(m_indent);
		return this;
	}

	public CodeBuilder Append(string text)
	{
		StringBuilder.Append(m_indent + text);
		return this;
	}

	public override string ToString()
		=> StringBuilder.ToString();

	public static implicit operator StringBuilder(CodeBuilder builder)
		=> builder.StringBuilder;
}
