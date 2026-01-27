using System.Runtime.CompilerServices;

namespace ChartTools.Extensions;

public static class UnsafeExtensions
{
	public static ref readonly TAs AsReadonly<T, TAs>(in T value)
		where T : unmanaged, Enum
		where TAs : unmanaged
	{
		ref T refVal = ref Unsafe.AsRef(in value);
		return ref Unsafe.As<T, TAs>(ref refVal);
	}
}
