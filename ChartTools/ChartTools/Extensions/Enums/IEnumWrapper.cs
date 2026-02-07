using System.Numerics;

namespace ChartTools.Extensions.Enums;

public interface IEnumWrapper<TSelf> :
	IEquatable<TSelf>, IEquatable<TSelf?>,
	IComparable<TSelf>,
	IEqualityOperators<TSelf, TSelf, bool>, IEqualityOperators<TSelf, TSelf?, bool>,
	IComparisonOperators<TSelf, TSelf, bool>,
	where TSelf : struct, IEnumWrapper<TSelf>
{
	static abstract bool operator ==(TSelf? left, TSelf right);

	static abstract bool operator ==(TSelf left, TSelf? right);

	static abstract bool operator !=(TSelf? left, TSelf right);

	static abstract bool operator !=(TSelf left, TSelf? right);
}

public interface IEnumWrapper<TSelf, TEnum> :
	IEquatable<TEnum>, IEquatable<TEnum?>,
	IComparable<TEnum>,
	IEqualityOperators<TSelf, TEnum, bool>, IEqualityOperators<TSelf, TEnum?, bool>,
	IComparisonOperators<TSelf, TEnum, bool>, IComparisonOperators<TSelf, TEnum?, bool>,
	IAdditionOperators<TSelf, TEnum, TEnum>, ISubtractionOperators<TSelf, TEnum, TEnum>,
	IBitwiseOperators<TSelf, TEnum, TEnum>, IShiftOperators<TSelf, TEnum, TEnum>,
	where TSelf : struct, IEnumWrapper<TSelf, TEnum>
	where TEnum : Enum
{
	TEnum Value { get; }

	static abstract implicit operator TEnum(TSelf wrapper);

	static abstract implicit operator TSelf(TEnum value);

	static abstract bool operator ==(TSelf left, TSelf? right);

	static abstract bool operator ==(TSelf? left, TSelf right);

	static abstract bool operator !=(TSelf left, TSelf? right);

	static abstract bool operator !=(TSelf? left, TSelf right);
}
