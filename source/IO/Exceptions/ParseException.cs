using System;

namespace ChartTools.IO
{
    public class ParseException : Exception
    {
        public string Object { get; }
        public string Target { get; }
        public Type Type { get; }

        public ParseException(string obj, string target, Type type) : base($"Cannot convert {target} \"{obj}\" to {type.Name}")
        {
            Object = obj;
            Target = target;
            Type = type;
        }
    }

}
