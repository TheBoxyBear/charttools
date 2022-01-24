using System;

namespace ChartTools
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class IniKeyAttribute : Attribute
    {
        public int Index { get; }
        public IniKeyAttribute(int index) => Index = index;
    }
}
