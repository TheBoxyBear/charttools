using System;

namespace ChartTools.Exceptions
{
    /// <summary>
    /// Exception thrown when a cosmic bit flip causes the world to collapse
    /// </summary>
    public class ImpossibleException : Exception
    {
        public ImpossibleException(string cause) : base($"Panic. ({cause})") { }
    }
}
