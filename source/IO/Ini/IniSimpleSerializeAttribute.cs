using System;

namespace ChartTools.IO.Ini
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class IniSimpleSerializeAttribute : Attribute
    {
        public string Key { get; }

        public IniSimpleSerializeAttribute(string key) => Key = key;
    }
}
