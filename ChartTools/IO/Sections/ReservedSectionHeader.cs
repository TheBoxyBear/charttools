﻿namespace ChartTools.IO.Sections;

public readonly struct ReservedSectionHeader
{
    public string Header { get; }
    public string DataSource { get; }

    public ReservedSectionHeader(string header, string dataSource)
    {
        Header = header;
        DataSource = dataSource;
    }
}
