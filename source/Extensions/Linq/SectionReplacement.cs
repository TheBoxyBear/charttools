using System;
using System.Collections.Generic;

namespace ChartTools.Extensions.Linq
{
    /// <summary>
    /// Replacement for a section of items in a collection
    /// </summary>
    /// <param name="Replacement">Items to replace with</param>
    /// <param name="StartReplace">Method that defines if a source marks the start of the section to replace</param>
    /// <param name="EndReplace">Method that defines if a source item marks the end of the section to replace</param>
    /// <param name="AddIfMissing">The replacement should be appended to the collection if the section to replace is not found</param>
    public readonly record struct SectionReplacement<T>(IEnumerable<T> Replacement, Predicate<T> StartReplace, Predicate<T> EndReplace, bool AddIfMissing);
}
