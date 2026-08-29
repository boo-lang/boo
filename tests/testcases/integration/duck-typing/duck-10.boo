"""
d_sbyte + d_sbyte != System.SByte ==> System.Int32
d_sbyte + d_byte != System.Byte ==> System.Int32
d_sbyte + d_short != System.Int16 ==> System.Int32
d_sbyte + d_ushort != System.UInt16 ==> System.Int32
d_sbyte + d_uint != System.UInt32 ==> System.Int64
d_sbyte + d_ulong != System.UInt64 ==> System.Int64
d_byte + d_byte != System.Byte ==> System.Int32
d_byte + d_short != System.Int16 ==> System.Int32
d_byte + d_ushort != System.UInt16 ==> System.Int32
d_short + d_short != System.Int16 ==> System.Int32
d_short + d_ushort != System.UInt16 ==> System.Int32
d_short + d_uint != System.UInt32 ==> System.Int64
d_short + d_ulong != System.UInt64 ==> System.Int64
d_ushort + d_ushort != System.UInt16 ==> System.Int32
d_int + d_uint != System.UInt32 ==> System.Int64
d_int + d_ulong != System.UInt64 ==> System.Int64
d_long + d_ulong != System.UInt64 ==> System.Int64
"""
_sbyte as System.SByte = cast(System.IConvertible, 2).ToSByte(null)
_byte as byte = 2
_short as short = 2
_ushort as ushort = 2
_int as int = 2
_uint as uint = 2
_long as long = 2
_ulong as ulong = 2
_single as single = 2
_double as double = 2
_decimal as System.Decimal = cast(System.IConvertible, 2).ToDecimal(null)
d_sbyte as duck = _sbyte
d_byte as duck = _byte
d_short as duck = _short
d_ushort as duck = _ushort
d_int as duck = _int
d_uint as duck = _uint
d_long as duck = _long
d_ulong as duck = _ulong
d_single as duck = _single
d_double as duck = _double
d_decimal as duck = _decimal

try:
	if ((d_sbyte + d_sbyte).GetType() != (d_sbyte).GetType()):
		System.Console.WriteLine("d_sbyte + d_sbyte != " + d_sbyte.GetType() + " ==> " + (d_sbyte + d_sbyte).GetType())
except e:
	System.Console.WriteLine("exception: d_sbyte + d_sbyte")
try:
	if ((d_sbyte + d_byte).GetType() != (d_byte).GetType()):
		System.Console.WriteLine("d_sbyte + d_byte != " + d_byte.GetType() + " ==> " + (d_sbyte + d_byte).GetType())
except e:
	System.Console.WriteLine("exception: d_sbyte + d_byte")
try:
	if ((d_sbyte + d_short).GetType() != (d_short).GetType()):
		System.Console.WriteLine("d_sbyte + d_short != " + d_short.GetType() + " ==> " + (d_sbyte + d_short).GetType())
except e:
	System.Console.WriteLine("exception: d_sbyte + d_short")
try:
	if ((d_sbyte + d_ushort).GetType() != (d_ushort).GetType()):
		System.Console.WriteLine("d_sbyte + d_ushort != " + d_ushort.GetType() + " ==> " + (d_sbyte + d_ushort).GetType())
except e:
	System.Console.WriteLine("exception: d_sbyte + d_ushort")
try:
	if ((d_sbyte + d_int).GetType() != (d_int).GetType()):
		System.Console.WriteLine("d_sbyte + d_int != " + d_int.GetType() + " ==> " + (d_sbyte + d_int).GetType())
except e:
	System.Console.WriteLine("exception: d_sbyte + d_int")
try:
	if ((d_sbyte + d_uint).GetType() != (d_uint).GetType()):
		System.Console.WriteLine("d_sbyte + d_uint != " + d_uint.GetType() + " ==> " + (d_sbyte + d_uint).GetType())
except e:
	System.Console.WriteLine("exception: d_sbyte + d_uint")
try:
	if ((d_sbyte + d_long).GetType() != (d_long).GetType()):
		System.Console.WriteLine("d_sbyte + d_long != " + d_long.GetType() + " ==> " + (d_sbyte + d_long).GetType())
except e:
	System.Console.WriteLine("exception: d_sbyte + d_long")
try:
	if ((d_sbyte + d_ulong).GetType() != (d_ulong).GetType()):
		System.Console.WriteLine("d_sbyte + d_ulong != " + d_ulong.GetType() + " ==> " + (d_sbyte + d_ulong).GetType())
except e:
	System.Console.WriteLine("exception: d_sbyte + d_ulong")
try:
	if ((d_sbyte + d_single).GetType() != (d_single).GetType()):
		System.Console.WriteLine("d_sbyte + d_single != " + d_single.GetType() + " ==> " + (d_sbyte + d_single).GetType())
except e:
	System.Console.WriteLine("exception: d_sbyte + d_single")
try:
	if ((d_sbyte + d_double).GetType() != (d_double).GetType()):
		System.Console.WriteLine("d_sbyte + d_double != " + d_double.GetType() + " ==> " + (d_sbyte + d_double).GetType())
except e:
	System.Console.WriteLine("exception: d_sbyte + d_double")
try:
	if ((d_sbyte + d_decimal).GetType() != (d_decimal).GetType()):
		System.Console.WriteLine("d_sbyte + d_decimal != " + d_decimal.GetType() + " ==> " + (d_sbyte + d_decimal).GetType())
except e:
	System.Console.WriteLine("exception: d_sbyte + d_decimal")
try:
	if ((d_byte + d_byte).GetType() != (d_byte).GetType()):
		System.Console.WriteLine("d_byte + d_byte != " + d_byte.GetType() + " ==> " + (d_byte + d_byte).GetType())
except e:
	System.Console.WriteLine("exception: d_byte + d_byte")
try:
	if ((d_byte + d_short).GetType() != (d_short).GetType()):
		System.Console.WriteLine("d_byte + d_short != " + d_short.GetType() + " ==> " + (d_byte + d_short).GetType())
except e:
	System.Console.WriteLine("exception: d_byte + d_short")
try:
	if ((d_byte + d_ushort).GetType() != (d_ushort).GetType()):
		System.Console.WriteLine("d_byte + d_ushort != " + d_ushort.GetType() + " ==> " + (d_byte + d_ushort).GetType())
except e:
	System.Console.WriteLine("exception: d_byte + d_ushort")
try:
	if ((d_byte + d_int).GetType() != (d_int).GetType()):
		System.Console.WriteLine("d_byte + d_int != " + d_int.GetType() + " ==> " + (d_byte + d_int).GetType())
except e:
	System.Console.WriteLine("exception: d_byte + d_int")
try:
	if ((d_byte + d_uint).GetType() != (d_uint).GetType()):
		System.Console.WriteLine("d_byte + d_uint != " + d_uint.GetType() + " ==> " + (d_byte + d_uint).GetType())
except e:
	System.Console.WriteLine("exception: d_byte + d_uint")
try:
	if ((d_byte + d_long).GetType() != (d_long).GetType()):
		System.Console.WriteLine("d_byte + d_long != " + d_long.GetType() + " ==> " + (d_byte + d_long).GetType())
except e:
	System.Console.WriteLine("exception: d_byte + d_long")
try:
	if ((d_byte + d_ulong).GetType() != (d_ulong).GetType()):
		System.Console.WriteLine("d_byte + d_ulong != " + d_ulong.GetType() + " ==> " + (d_byte + d_ulong).GetType())
except e:
	System.Console.WriteLine("exception: d_byte + d_ulong")
try:
	if ((d_byte + d_single).GetType() != (d_single).GetType()):
		System.Console.WriteLine("d_byte + d_single != " + d_single.GetType() + " ==> " + (d_byte + d_single).GetType())
except e:
	System.Console.WriteLine("exception: d_byte + d_single")
try:
	if ((d_byte + d_double).GetType() != (d_double).GetType()):
		System.Console.WriteLine("d_byte + d_double != " + d_double.GetType() + " ==> " + (d_byte + d_double).GetType())
except e:
	System.Console.WriteLine("exception: d_byte + d_double")
try:
	if ((d_byte + d_decimal).GetType() != (d_decimal).GetType()):
		System.Console.WriteLine("d_byte + d_decimal != " + d_decimal.GetType() + " ==> " + (d_byte + d_decimal).GetType())
except e:
	System.Console.WriteLine("exception: d_byte + d_decimal")
try:
	if ((d_short + d_short).GetType() != (d_short).GetType()):
		System.Console.WriteLine("d_short + d_short != " + d_short.GetType() + " ==> " + (d_short + d_short).GetType())
except e:
	System.Console.WriteLine("exception: d_short + d_short")
try:
	if ((d_short + d_ushort).GetType() != (d_ushort).GetType()):
		System.Console.WriteLine("d_short + d_ushort != " + d_ushort.GetType() + " ==> " + (d_short + d_ushort).GetType())
except e:
	System.Console.WriteLine("exception: d_short + d_ushort")
try:
	if ((d_short + d_int).GetType() != (d_int).GetType()):
		System.Console.WriteLine("d_short + d_int != " + d_int.GetType() + " ==> " + (d_short + d_int).GetType())
except e:
	System.Console.WriteLine("exception: d_short + d_int")
try:
	if ((d_short + d_uint).GetType() != (d_uint).GetType()):
		System.Console.WriteLine("d_short + d_uint != " + d_uint.GetType() + " ==> " + (d_short + d_uint).GetType())
except e:
	System.Console.WriteLine("exception: d_short + d_uint")
try:
	if ((d_short + d_long).GetType() != (d_long).GetType()):
		System.Console.WriteLine("d_short + d_long != " + d_long.GetType() + " ==> " + (d_short + d_long).GetType())
except e:
	System.Console.WriteLine("exception: d_short + d_long")
try:
	if ((d_short + d_ulong).GetType() != (d_ulong).GetType()):
		System.Console.WriteLine("d_short + d_ulong != " + d_ulong.GetType() + " ==> " + (d_short + d_ulong).GetType())
except e:
	System.Console.WriteLine("exception: d_short + d_ulong")
try:
	if ((d_short + d_single).GetType() != (d_single).GetType()):
		System.Console.WriteLine("d_short + d_single != " + d_single.GetType() + " ==> " + (d_short + d_single).GetType())
except e:
	System.Console.WriteLine("exception: d_short + d_single")
try:
	if ((d_short + d_double).GetType() != (d_double).GetType()):
		System.Console.WriteLine("d_short + d_double != " + d_double.GetType() + " ==> " + (d_short + d_double).GetType())
except e:
	System.Console.WriteLine("exception: d_short + d_double")
try:
	if ((d_short + d_decimal).GetType() != (d_decimal).GetType()):
		System.Console.WriteLine("d_short + d_decimal != " + d_decimal.GetType() + " ==> " + (d_short + d_decimal).GetType())
except e:
	System.Console.WriteLine("exception: d_short + d_decimal")
try:
	if ((d_ushort + d_ushort).GetType() != (d_ushort).GetType()):
		System.Console.WriteLine("d_ushort + d_ushort != " + d_ushort.GetType() + " ==> " + (d_ushort + d_ushort).GetType())
except e:
	System.Console.WriteLine("exception: d_ushort + d_ushort")
try:
	if ((d_ushort + d_int).GetType() != (d_int).GetType()):
		System.Console.WriteLine("d_ushort + d_int != " + d_int.GetType() + " ==> " + (d_ushort + d_int).GetType())
except e:
	System.Console.WriteLine("exception: d_ushort + d_int")
try:
	if ((d_ushort + d_uint).GetType() != (d_uint).GetType()):
		System.Console.WriteLine("d_ushort + d_uint != " + d_uint.GetType() + " ==> " + (d_ushort + d_uint).GetType())
except e:
	System.Console.WriteLine("exception: d_ushort + d_uint")
try:
	if ((d_ushort + d_long).GetType() != (d_long).GetType()):
		System.Console.WriteLine("d_ushort + d_long != " + d_long.GetType() + " ==> " + (d_ushort + d_long).GetType())
except e:
	System.Console.WriteLine("exception: d_ushort + d_long")
try:
	if ((d_ushort + d_ulong).GetType() != (d_ulong).GetType()):
		System.Console.WriteLine("d_ushort + d_ulong != " + d_ulong.GetType() + " ==> " + (d_ushort + d_ulong).GetType())
except e:
	System.Console.WriteLine("exception: d_ushort + d_ulong")
try:
	if ((d_ushort + d_single).GetType() != (d_single).GetType()):
		System.Console.WriteLine("d_ushort + d_single != " + d_single.GetType() + " ==> " + (d_ushort + d_single).GetType())
except e:
	System.Console.WriteLine("exception: d_ushort + d_single")
try:
	if ((d_ushort + d_double).GetType() != (d_double).GetType()):
		System.Console.WriteLine("d_ushort + d_double != " + d_double.GetType() + " ==> " + (d_ushort + d_double).GetType())
except e:
	System.Console.WriteLine("exception: d_ushort + d_double")
try:
	if ((d_ushort + d_decimal).GetType() != (d_decimal).GetType()):
		System.Console.WriteLine("d_ushort + d_decimal != " + d_decimal.GetType() + " ==> " + (d_ushort + d_decimal).GetType())
except e:
	System.Console.WriteLine("exception: d_ushort + d_decimal")
try:
	if ((d_int + d_int).GetType() != (d_int).GetType()):
		System.Console.WriteLine("d_int + d_int != " + d_int.GetType() + " ==> " + (d_int + d_int).GetType())
except e:
	System.Console.WriteLine("exception: d_int + d_int")
try:
	if ((d_int + d_uint).GetType() != (d_uint).GetType()):
		System.Console.WriteLine("d_int + d_uint != " + d_uint.GetType() + " ==> " + (d_int + d_uint).GetType())
except e:
	System.Console.WriteLine("exception: d_int + d_uint")
try:
	if ((d_int + d_long).GetType() != (d_long).GetType()):
		System.Console.WriteLine("d_int + d_long != " + d_long.GetType() + " ==> " + (d_int + d_long).GetType())
except e:
	System.Console.WriteLine("exception: d_int + d_long")
try:
	if ((d_int + d_ulong).GetType() != (d_ulong).GetType()):
		System.Console.WriteLine("d_int + d_ulong != " + d_ulong.GetType() + " ==> " + (d_int + d_ulong).GetType())
except e:
	System.Console.WriteLine("exception: d_int + d_ulong")
try:
	if ((d_int + d_single).GetType() != (d_single).GetType()):
		System.Console.WriteLine("d_int + d_single != " + d_single.GetType() + " ==> " + (d_int + d_single).GetType())
except e:
	System.Console.WriteLine("exception: d_int + d_single")
try:
	if ((d_int + d_double).GetType() != (d_double).GetType()):
		System.Console.WriteLine("d_int + d_double != " + d_double.GetType() + " ==> " + (d_int + d_double).GetType())
except e:
	System.Console.WriteLine("exception: d_int + d_double")
try:
	if ((d_int + d_decimal).GetType() != (d_decimal).GetType()):
		System.Console.WriteLine("d_int + d_decimal != " + d_decimal.GetType() + " ==> " + (d_int + d_decimal).GetType())
except e:
	System.Console.WriteLine("exception: d_int + d_decimal")
try:
	if ((d_uint + d_uint).GetType() != (d_uint).GetType()):
		System.Console.WriteLine("d_uint + d_uint != " + d_uint.GetType() + " ==> " + (d_uint + d_uint).GetType())
except e:
	System.Console.WriteLine("exception: d_uint + d_uint")
try:
	if ((d_uint + d_long).GetType() != (d_long).GetType()):
		System.Console.WriteLine("d_uint + d_long != " + d_long.GetType() + " ==> " + (d_uint + d_long).GetType())
except e:
	System.Console.WriteLine("exception: d_uint + d_long")
try:
	if ((d_uint + d_ulong).GetType() != (d_ulong).GetType()):
		System.Console.WriteLine("d_uint + d_ulong != " + d_ulong.GetType() + " ==> " + (d_uint + d_ulong).GetType())
except e:
	System.Console.WriteLine("exception: d_uint + d_ulong")
try:
	if ((d_uint + d_single).GetType() != (d_single).GetType()):
		System.Console.WriteLine("d_uint + d_single != " + d_single.GetType() + " ==> " + (d_uint + d_single).GetType())
except e:
	System.Console.WriteLine("exception: d_uint + d_single")
try:
	if ((d_uint + d_double).GetType() != (d_double).GetType()):
		System.Console.WriteLine("d_uint + d_double != " + d_double.GetType() + " ==> " + (d_uint + d_double).GetType())
except e:
	System.Console.WriteLine("exception: d_uint + d_double")
try:
	if ((d_uint + d_decimal).GetType() != (d_decimal).GetType()):
		System.Console.WriteLine("d_uint + d_decimal != " + d_decimal.GetType() + " ==> " + (d_uint + d_decimal).GetType())
except e:
	System.Console.WriteLine("exception: d_uint + d_decimal")
try:
	if ((d_long + d_long).GetType() != (d_long).GetType()):
		System.Console.WriteLine("d_long + d_long != " + d_long.GetType() + " ==> " + (d_long + d_long).GetType())
except e:
	System.Console.WriteLine("exception: d_long + d_long")
try:
	if ((d_long + d_ulong).GetType() != (d_ulong).GetType()):
		System.Console.WriteLine("d_long + d_ulong != " + d_ulong.GetType() + " ==> " + (d_long + d_ulong).GetType())
except e:
	System.Console.WriteLine("exception: d_long + d_ulong")
try:
	if ((d_long + d_single).GetType() != (d_single).GetType()):
		System.Console.WriteLine("d_long + d_single != " + d_single.GetType() + " ==> " + (d_long + d_single).GetType())
except e:
	System.Console.WriteLine("exception: d_long + d_single")
try:
	if ((d_long + d_double).GetType() != (d_double).GetType()):
		System.Console.WriteLine("d_long + d_double != " + d_double.GetType() + " ==> " + (d_long + d_double).GetType())
except e:
	System.Console.WriteLine("exception: d_long + d_double")
try:
	if ((d_long + d_decimal).GetType() != (d_decimal).GetType()):
		System.Console.WriteLine("d_long + d_decimal != " + d_decimal.GetType() + " ==> " + (d_long + d_decimal).GetType())
except e:
	System.Console.WriteLine("exception: d_long + d_decimal")
try:
	if ((d_ulong + d_ulong).GetType() != (d_ulong).GetType()):
		System.Console.WriteLine("d_ulong + d_ulong != " + d_ulong.GetType() + " ==> " + (d_ulong + d_ulong).GetType())
except e:
	System.Console.WriteLine("exception: d_ulong + d_ulong")
try:
	if ((d_ulong + d_single).GetType() != (d_single).GetType()):
		System.Console.WriteLine("d_ulong + d_single != " + d_single.GetType() + " ==> " + (d_ulong + d_single).GetType())
except e:
	System.Console.WriteLine("exception: d_ulong + d_single")
try:
	if ((d_ulong + d_double).GetType() != (d_double).GetType()):
		System.Console.WriteLine("d_ulong + d_double != " + d_double.GetType() + " ==> " + (d_ulong + d_double).GetType())
except e:
	System.Console.WriteLine("exception: d_ulong + d_double")
try:
	if ((d_ulong + d_decimal).GetType() != (d_decimal).GetType()):
		System.Console.WriteLine("d_ulong + d_decimal != " + d_decimal.GetType() + " ==> " + (d_ulong + d_decimal).GetType())
except e:
	System.Console.WriteLine("exception: d_ulong + d_decimal")
try:
	if ((d_single + d_single).GetType() != (d_single).GetType()):
		System.Console.WriteLine("d_single + d_single != " + d_single.GetType() + " ==> " + (d_single + d_single).GetType())
except e:
	System.Console.WriteLine("exception: d_single + d_single")
try:
	if ((d_single + d_double).GetType() != (d_double).GetType()):
		System.Console.WriteLine("d_single + d_double != " + d_double.GetType() + " ==> " + (d_single + d_double).GetType())
except e:
	System.Console.WriteLine("exception: d_single + d_double")
try:
	if ((d_single + d_decimal).GetType() != (d_decimal).GetType()):
		System.Console.WriteLine("d_single + d_decimal != " + d_decimal.GetType() + " ==> " + (d_single + d_decimal).GetType())
except e:
	System.Console.WriteLine("exception: d_single + d_decimal")
try:
	if ((d_double + d_double).GetType() != (d_double).GetType()):
		System.Console.WriteLine("d_double + d_double != " + d_double.GetType() + " ==> " + (d_double + d_double).GetType())
except e:
	System.Console.WriteLine("exception: d_double + d_double")
try:
	if ((d_double + d_decimal).GetType() != (d_decimal).GetType()):
		System.Console.WriteLine("d_double + d_decimal != " + d_decimal.GetType() + " ==> " + (d_double + d_decimal).GetType())
except e:
	System.Console.WriteLine("exception: d_double + d_decimal")
try:
	if ((d_decimal + d_decimal).GetType() != (d_decimal).GetType()):
		System.Console.WriteLine("d_decimal + d_decimal != " + d_decimal.GetType() + " ==> " + (d_decimal + d_decimal).GetType())
except e:
	System.Console.WriteLine("exception: d_decimal + d_decimal")
