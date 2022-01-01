using ChartTools.IO.Chart.Sessions;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools.IO.Chart.Serializers
{
    internal abstract class ChartSerializer<T> : ChartSerializer
    {
        public T Content { get; }

        public ChartSerializer(string header, T content) : base(header) => Content = content;
    }
}
