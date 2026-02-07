using System.Runtime.CompilerServices;

namespace ChartTools.Extensions;

public static class UnsafeExtensions
{
	public static ref readonly TAs AsRefReadonly<T, TAs>(in T value)
	{
		ref T refVal = ref Unsafe.AsRef(in value);
		return ref Unsafe.As<T, TAs>(ref refVal);
	}
}
