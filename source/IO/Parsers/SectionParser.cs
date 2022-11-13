﻿using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Parsing;

internal abstract class SectionParser<T> : FileParser<T>
{
    public string Header { get; }

    public SectionParser(ReadingSession session, string header) : base(session) => Header = header;

    protected override Exception GetHandleException(T item, Exception innerException) => new SectionException(Header, GetHandleInnerException(item, innerException));
    protected abstract Exception GetHandleInnerException(T item, Exception innerException);
    protected override Exception GetFinalizeException(Exception innerException) => new SectionException(Header, innerException);
}
