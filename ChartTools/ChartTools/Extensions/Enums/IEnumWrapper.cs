#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace ChartTools.Extensions.Enums;

public interface IEnumWrapper<TSelf> :
	IEquatable<TSelf>, IEquatable<TSelf?>,
	IComparable<TSelf>
#if NET7_0_OR_GREATER
	,IEqualityOperators<TSelf, TSelf, bool>, IEqualityOperators<TSelf, TSelf?, bool>,
	IComparisonOperators<TSelf, TSelf, bool>,
	IParsable<TSelf>, ISpanParsable<TSelf>
#endif
	where TSelf : struct, IEnumWrapper<TSelf>
{ }

public interface IEnumWrapper<TSelf, TEnum> : IEnumWrapper<TSelf>
	where TSelf : struct, IEnumWrapper<TSelf, TEnum>,
	IEquatable<TEnum>, IEquatable<TEnum?>,
	IComparable<TEnum>
#if NET7_0_OR_GREATER
	,IEqualityOperators<TSelf, TEnum, bool>, IEqualityOperators<TSelf, TEnum?, bool>,
	IComparisonOperators<TSelf, TSelf, bool>,
	IBitwiseOperators<TSelf, TEnum, TEnum>
#endif
	where TEnum : struct, Enum
{
	TEnum Value { get; }

#if NET7_0_OR_GREATER
	static abstract implicit operator TEnum(TSelf wrapper);

	static abstract implicit operator TSelf(TEnum value);
#endif
}
