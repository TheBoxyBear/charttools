using System.Numerics;

namespace ChartTools.Extensions.Enums;

public interface IEnumWrapper<TSelf> :
	IEquatable<TSelf>, IEquatable<TSelf?>,
	IComparable<TSelf>,
	IEqualityOperators<TSelf, TSelf, bool>, IEqualityOperators<TSelf, TSelf?, bool>,
	IComparisonOperators<TSelf, TSelf, bool>
	where TSelf : struct, IEnumWrapper<TSelf>
{ }

public interface IEnumWrapper<TSelf, TEnum> : IEnumWrapper<TSelf>,
	IBitwiseOperators<TSelf, TSelf, TEnum>,
	IShiftOperators<TSelf, int, TEnum>
	where TSelf : struct, IEnumWrapper<TSelf, TEnum>
	where TEnum : struct, Enum
{
	TEnum Value { get; }

	static abstract implicit operator TEnum(TSelf wrapper);

	static abstract implicit operator TSelf(TEnum value);
}
