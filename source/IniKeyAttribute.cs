using System;

namespace ChartTools
{
    internal class IniKeyAttribute : Attribute
    {
        public string Key { get; }
        public IniKeyAttribute(string key) => Key = key;
    }
}
