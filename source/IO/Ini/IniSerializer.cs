using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Ini
{
    internal class IniSerializer : Serializer<string>
    {
        public IniSerializer(string header, WritingSession session) : base(header, session) { }

        public override IEnumerable<string> Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
