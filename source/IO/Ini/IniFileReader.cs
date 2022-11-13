﻿namespace ChartTools.IO.Ini;

internal class IniFileReader : TextFileReader
{
    public override IEnumerable<IniParser> Parsers => base.Parsers.Cast<IniParser>();

    public IniFileReader(string path, Func<string, IniParser?> parserGetter) : base(path, parserGetter) { }

    protected override bool IsSectionStart(string line) => !line.StartsWith('[');
}
