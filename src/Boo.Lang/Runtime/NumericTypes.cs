using System;

namespace Boo.Lang.Runtime
{
	public class NumericTypes
	{
		public static bool IsWideningPromotion(Type paramType, Type argType)
		{
			return NumericRangeOrder(paramType) > NumericRangeOrder(argType);
		}

		public static int NumericRangeOrder(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
					return 1;
				case TypeCode.Byte:
				case TypeCode.SByte:
					return 2;
				case TypeCode.Int16:
				case TypeCode.Char:
				case TypeCode.UInt16:
					return 3;
				case TypeCode.Int32:
				case TypeCode.UInt32:
					return 4;
				case TypeCode.Int64:
				case TypeCode.UInt64:
					return 5;
				case TypeCode.Decimal:
					return 6;
				case TypeCode.Single:
					return 7;
				case TypeCode.Double:
					return 8;
			}
			throw new ArgumentException(type.ToString());
		}
	}
}
