using System;

namespace ChartTools.Extensions
{
    /// <summary>
    /// <see cref="IEquatable{T}"/> equivalent to the <see cref="IComparable{T}"/> <see cref="Comparison{T}"/> delegate
    /// </summary>
    public delegate bool EqualityComparison<in T>(T a, T b);
}
