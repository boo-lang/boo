"""
_sbyte + _sbyte != System.SByte ==> System.Int32
_sbyte + _byte != System.Byte ==> System.Int32
_sbyte + _short != System.Int16 ==> System.Int32
_sbyte + _ushort != System.UInt16 ==> System.Int32
_sbyte + _uint != System.UInt32 ==> System.Int64
_sbyte + _ulong != System.UInt64 ==> System.Int64
_byte + _byte != System.Byte ==> System.Int32
_byte + _short != System.Int16 ==> System.Int32
_byte + _ushort != System.UInt16 ==> System.Int32
_short + _short != System.Int16 ==> System.Int32
_short + _ushort != System.UInt16 ==> System.Int32
_short + _uint != System.UInt32 ==> System.Int64
_short + _ulong != System.UInt64 ==> System.Int64
_ushort + _ushort != System.UInt16 ==> System.Int32
_int + _uint != System.UInt32 ==> System.Int64
_int + _ulong != System.UInt64 ==> System.Int64
_long + _ulong != System.UInt64 ==> System.Int64
"""
_sbyte as sbyte = 2
_byte as byte = 2
_short as short = 2
_ushort as ushort = 2
_int as int = 2
_uint as uint = 2
_long as long = 2
_ulong as ulong = 2
_single as single = 2
_double as double = 2
try:
	if ((_sbyte + _sbyte).GetType() != (_sbyte).GetType()):
		System.Console.WriteLine("_sbyte + _sbyte != " + _sbyte.GetType() + " ==> " + (_sbyte + _sbyte).GetType())
except e:
	System.Console.WriteLine("exception: _sbyte + _sbyte")
try:
	if ((_sbyte + _byte).GetType() != (_byte).GetType()):
		System.Console.WriteLine("_sbyte + _byte != " + _byte.GetType() + " ==> " + (_sbyte + _byte).GetType())
except e:
	System.Console.WriteLine("exception: _sbyte + _byte")
try:
	if ((_sbyte + _short).GetType() != (_short).GetType()):
		System.Console.WriteLine("_sbyte + _short != " + _short.GetType() + " ==> " + (_sbyte + _short).GetType())
except e:
	System.Console.WriteLine("exception: _sbyte + _short")
try:
	if ((_sbyte + _ushort).GetType() != (_ushort).GetType()):
		System.Console.WriteLine("_sbyte + _ushort != " + _ushort.GetType() + " ==> " + (_sbyte + _ushort).GetType())
except e:
	System.Console.WriteLine("exception: _sbyte + _ushort")
try:
	if ((_sbyte + _int).GetType() != (_int).GetType()):
		System.Console.WriteLine("_sbyte + _int != " + _int.GetType() + " ==> " + (_sbyte + _int).GetType())
except e:
	System.Console.WriteLine("exception: _sbyte + _int")
try:
	if ((_sbyte + _uint).GetType() != (_uint).GetType()):
		System.Console.WriteLine("_sbyte + _uint != " + _uint.GetType() + " ==> " + (_sbyte + _uint).GetType())
except e:
	System.Console.WriteLine("exception: _sbyte + _uint")
try:
	if ((_sbyte + _long).GetType() != (_long).GetType()):
		System.Console.WriteLine("_sbyte + _long != " + _long.GetType() + " ==> " + (_sbyte + _long).GetType())
except e:
	System.Console.WriteLine("exception: _sbyte + _long")
try:
	if ((_sbyte + _ulong).GetType() != (_ulong).GetType()):
		System.Console.WriteLine("_sbyte + _ulong != " + _ulong.GetType() + " ==> " + (_sbyte + _ulong).GetType())
except e:
	System.Console.WriteLine("exception: _sbyte + _ulong")
try:
	if ((_sbyte + _single).GetType() != (_single).GetType()):
		System.Console.WriteLine("_sbyte + _single != " + _single.GetType() + " ==> " + (_sbyte + _single).GetType())
except e:
	System.Console.WriteLine("exception: _sbyte + _single")
try:
	if ((_sbyte + _double).GetType() != (_double).GetType()):
		System.Console.WriteLine("_sbyte + _double != " + _double.GetType() + " ==> " + (_sbyte + _double).GetType())
except e:
	System.Console.WriteLine("exception: _sbyte + _double")
try:
	if ((_byte + _byte).GetType() != (_byte).GetType()):
		System.Console.WriteLine("_byte + _byte != " + _byte.GetType() + " ==> " + (_byte + _byte).GetType())
except e:
	System.Console.WriteLine("exception: _byte + _byte")
try:
	if ((_byte + _short).GetType() != (_short).GetType()):
		System.Console.WriteLine("_byte + _short != " + _short.GetType() + " ==> " + (_byte + _short).GetType())
except e:
	System.Console.WriteLine("exception: _byte + _short")
try:
	if ((_byte + _ushort).GetType() != (_ushort).GetType()):
		System.Console.WriteLine("_byte + _ushort != " + _ushort.GetType() + " ==> " + (_byte + _ushort).GetType())
except e:
	System.Console.WriteLine("exception: _byte + _ushort")
try:
	if ((_byte + _int).GetType() != (_int).GetType()):
		System.Console.WriteLine("_byte + _int != " + _int.GetType() + " ==> " + (_byte + _int).GetType())
except e:
	System.Console.WriteLine("exception: _byte + _int")
try:
	if ((_byte + _uint).GetType() != (_uint).GetType()):
		System.Console.WriteLine("_byte + _uint != " + _uint.GetType() + " ==> " + (_byte + _uint).GetType())
except e:
	System.Console.WriteLine("exception: _byte + _uint")
try:
	if ((_byte + _long).GetType() != (_long).GetType()):
		System.Console.WriteLine("_byte + _long != " + _long.GetType() + " ==> " + (_byte + _long).GetType())
except e:
	System.Console.WriteLine("exception: _byte + _long")
try:
	if ((_byte + _ulong).GetType() != (_ulong).GetType()):
		System.Console.WriteLine("_byte + _ulong != " + _ulong.GetType() + " ==> " + (_byte + _ulong).GetType())
except e:
	System.Console.WriteLine("exception: _byte + _ulong")
try:
	if ((_byte + _single).GetType() != (_single).GetType()):
		System.Console.WriteLine("_byte + _single != " + _single.GetType() + " ==> " + (_byte + _single).GetType())
except e:
	System.Console.WriteLine("exception: _byte + _single")
try:
	if ((_byte + _double).GetType() != (_double).GetType()):
		System.Console.WriteLine("_byte + _double != " + _double.GetType() + " ==> " + (_byte + _double).GetType())
except e:
	System.Console.WriteLine("exception: _byte + _double")
try:
	if ((_short + _short).GetType() != (_short).GetType()):
		System.Console.WriteLine("_short + _short != " + _short.GetType() + " ==> " + (_short + _short).GetType())
except e:
	System.Console.WriteLine("exception: _short + _short")
try:
	if ((_short + _ushort).GetType() != (_ushort).GetType()):
		System.Console.WriteLine("_short + _ushort != " + _ushort.GetType() + " ==> " + (_short + _ushort).GetType())
except e:
	System.Console.WriteLine("exception: _short + _ushort")
try:
	if ((_short + _int).GetType() != (_int).GetType()):
		System.Console.WriteLine("_short + _int != " + _int.GetType() + " ==> " + (_short + _int).GetType())
except e:
	System.Console.WriteLine("exception: _short + _int")
try:
	if ((_short + _uint).GetType() != (_uint).GetType()):
		System.Console.WriteLine("_short + _uint != " + _uint.GetType() + " ==> " + (_short + _uint).GetType())
except e:
	System.Console.WriteLine("exception: _short + _uint")
try:
	if ((_short + _long).GetType() != (_long).GetType()):
		System.Console.WriteLine("_short + _long != " + _long.GetType() + " ==> " + (_short + _long).GetType())
except e:
	System.Console.WriteLine("exception: _short + _long")
try:
	if ((_short + _ulong).GetType() != (_ulong).GetType()):
		System.Console.WriteLine("_short + _ulong != " + _ulong.GetType() + " ==> " + (_short + _ulong).GetType())
except e:
	System.Console.WriteLine("exception: _short + _ulong")
try:
	if ((_short + _single).GetType() != (_single).GetType()):
		System.Console.WriteLine("_short + _single != " + _single.GetType() + " ==> " + (_short + _single).GetType())
except e:
	System.Console.WriteLine("exception: _short + _single")
try:
	if ((_short + _double).GetType() != (_double).GetType()):
		System.Console.WriteLine("_short + _double != " + _double.GetType() + " ==> " + (_short + _double).GetType())
except e:
	System.Console.WriteLine("exception: _short + _double")
try:
	if ((_ushort + _ushort).GetType() != (_ushort).GetType()):
		System.Console.WriteLine("_ushort + _ushort != " + _ushort.GetType() + " ==> " + (_ushort + _ushort).GetType())
except e:
	System.Console.WriteLine("exception: _ushort + _ushort")
try:
	if ((_ushort + _int).GetType() != (_int).GetType()):
		System.Console.WriteLine("_ushort + _int != " + _int.GetType() + " ==> " + (_ushort + _int).GetType())
except e:
	System.Console.WriteLine("exception: _ushort + _int")
try:
	if ((_ushort + _uint).GetType() != (_uint).GetType()):
		System.Console.WriteLine("_ushort + _uint != " + _uint.GetType() + " ==> " + (_ushort + _uint).GetType())
except e:
	System.Console.WriteLine("exception: _ushort + _uint")
try:
	if ((_ushort + _long).GetType() != (_long).GetType()):
		System.Console.WriteLine("_ushort + _long != " + _long.GetType() + " ==> " + (_ushort + _long).GetType())
except e:
	System.Console.WriteLine("exception: _ushort + _long")
try:
	if ((_ushort + _ulong).GetType() != (_ulong).GetType()):
		System.Console.WriteLine("_ushort + _ulong != " + _ulong.GetType() + " ==> " + (_ushort + _ulong).GetType())
except e:
	System.Console.WriteLine("exception: _ushort + _ulong")
try:
	if ((_ushort + _single).GetType() != (_single).GetType()):
		System.Console.WriteLine("_ushort + _single != " + _single.GetType() + " ==> " + (_ushort + _single).GetType())
except e:
	System.Console.WriteLine("exception: _ushort + _single")
try:
	if ((_ushort + _double).GetType() != (_double).GetType()):
		System.Console.WriteLine("_ushort + _double != " + _double.GetType() + " ==> " + (_ushort + _double).GetType())
except e:
	System.Console.WriteLine("exception: _ushort + _double")
try:
	if ((_int + _int).GetType() != (_int).GetType()):
		System.Console.WriteLine("_int + _int != " + _int.GetType() + " ==> " + (_int + _int).GetType())
except e:
	System.Console.WriteLine("exception: _int + _int")
try:
	if ((_int + _uint).GetType() != (_uint).GetType()):
		System.Console.WriteLine("_int + _uint != " + _uint.GetType() + " ==> " + (_int + _uint).GetType())
except e:
	System.Console.WriteLine("exception: _int + _uint")
try:
	if ((_int + _long).GetType() != (_long).GetType()):
		System.Console.WriteLine("_int + _long != " + _long.GetType() + " ==> " + (_int + _long).GetType())
except e:
	System.Console.WriteLine("exception: _int + _long")
try:
	if ((_int + _ulong).GetType() != (_ulong).GetType()):
		System.Console.WriteLine("_int + _ulong != " + _ulong.GetType() + " ==> " + (_int + _ulong).GetType())
except e:
	System.Console.WriteLine("exception: _int + _ulong")
try:
	if ((_int + _single).GetType() != (_single).GetType()):
		System.Console.WriteLine("_int + _single != " + _single.GetType() + " ==> " + (_int + _single).GetType())
except e:
	System.Console.WriteLine("exception: _int + _single")
try:
	if ((_int + _double).GetType() != (_double).GetType()):
		System.Console.WriteLine("_int + _double != " + _double.GetType() + " ==> " + (_int + _double).GetType())
except e:
	System.Console.WriteLine("exception: _int + _double")
try:
	if ((_uint + _uint).GetType() != (_uint).GetType()):
		System.Console.WriteLine("_uint + _uint != " + _uint.GetType() + " ==> " + (_uint + _uint).GetType())
except e:
	System.Console.WriteLine("exception: _uint + _uint")
try:
	if ((_uint + _long).GetType() != (_long).GetType()):
		System.Console.WriteLine("_uint + _long != " + _long.GetType() + " ==> " + (_uint + _long).GetType())
except e:
	System.Console.WriteLine("exception: _uint + _long")
try:
	if ((_uint + _ulong).GetType() != (_ulong).GetType()):
		System.Console.WriteLine("_uint + _ulong != " + _ulong.GetType() + " ==> " + (_uint + _ulong).GetType())
except e:
	System.Console.WriteLine("exception: _uint + _ulong")
try:
	if ((_uint + _single).GetType() != (_single).GetType()):
		System.Console.WriteLine("_uint + _single != " + _single.GetType() + " ==> " + (_uint + _single).GetType())
except e:
	System.Console.WriteLine("exception: _uint + _single")
try:
	if ((_uint + _double).GetType() != (_double).GetType()):
		System.Console.WriteLine("_uint + _double != " + _double.GetType() + " ==> " + (_uint + _double).GetType())
except e:
	System.Console.WriteLine("exception: _uint + _double")
try:
	if ((_long + _long).GetType() != (_long).GetType()):
		System.Console.WriteLine("_long + _long != " + _long.GetType() + " ==> " + (_long + _long).GetType())
except e:
	System.Console.WriteLine("exception: _long + _long")
try:
	if ((_long + _ulong).GetType() != (_ulong).GetType()):
		System.Console.WriteLine("_long + _ulong != " + _ulong.GetType() + " ==> " + (_long + _ulong).GetType())
except e:
	System.Console.WriteLine("exception: _long + _ulong")
try:
	if ((_long + _single).GetType() != (_single).GetType()):
		System.Console.WriteLine("_long + _single != " + _single.GetType() + " ==> " + (_long + _single).GetType())
except e:
	System.Console.WriteLine("exception: _long + _single")
try:
	if ((_long + _double).GetType() != (_double).GetType()):
		System.Console.WriteLine("_long + _double != " + _double.GetType() + " ==> " + (_long + _double).GetType())
except e:
	System.Console.WriteLine("exception: _long + _double")
try:
	if ((_ulong + _ulong).GetType() != (_ulong).GetType()):
		System.Console.WriteLine("_ulong + _ulong != " + _ulong.GetType() + " ==> " + (_ulong + _ulong).GetType())
except e:
	System.Console.WriteLine("exception: _ulong + _ulong")
try:
	if ((_ulong + _single).GetType() != (_single).GetType()):
		System.Console.WriteLine("_ulong + _single != " + _single.GetType() + " ==> " + (_ulong + _single).GetType())
except e:
	System.Console.WriteLine("exception: _ulong + _single")
try:
	if ((_ulong + _double).GetType() != (_double).GetType()):
		System.Console.WriteLine("_ulong + _double != " + _double.GetType() + " ==> " + (_ulong + _double).GetType())
except e:
	System.Console.WriteLine("exception: _ulong + _double")
try:
	if ((_single + _single).GetType() != (_single).GetType()):
		System.Console.WriteLine("_single + _single != " + _single.GetType() + " ==> " + (_single + _single).GetType())
except e:
	System.Console.WriteLine("exception: _single + _single")
try:
	if ((_single + _double).GetType() != (_double).GetType()):
		System.Console.WriteLine("_single + _double != " + _double.GetType() + " ==> " + (_single + _double).GetType())
except e:
	System.Console.WriteLine("exception: _single + _double")
try:
	if ((_double + _double).GetType() != (_double).GetType()):
		System.Console.WriteLine("_double + _double != " + _double.GetType() + " ==> " + (_double + _double).GetType())
except e:
	System.Console.WriteLine("exception: _double + _double")
