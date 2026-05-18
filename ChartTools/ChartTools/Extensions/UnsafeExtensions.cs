using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChartTools.Extensions;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class UnsafeExtensions
{
	extension(Unsafe)
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly TAs As<T, TAs>(in T value)
		{
			ref T refVal = ref Unsafe.AsRef(in value);
			return ref Unsafe.As<T, TAs>(ref refVal);
		}
	}
}
