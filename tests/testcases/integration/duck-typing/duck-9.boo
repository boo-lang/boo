_bool as bool = true
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
d_bool as duck = _bool
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

assert 10.Equals(d_ushort + 8)
assert 12.0.Equals(d_double*7L - d_long)
assert 16.Equals(d_sbyte * 8)
assert 0.Equals(d_byte / 3)
assert 0 == ((d_short + (3 * d_uint)) % d_byte)
assert 0L.Equals(((d_short + (3 * d_uint)) % d_byte))
assert 8.0.Equals(d_int ** (d_decimal + 1))
assert d_int >= d_double
assert false == (d_int >= d_double + 1)
assert d_int >= d_double - 1
assert d_int <= d_double
assert d_int <= d_int + 1
assert false == (d_int <= d_int - 1)
assert false == (d_int > d_double)
assert false == (d_int > d_ushort)
assert d_int > d_double - 1
assert false == (d_uint < d_single)
assert d_int < d_int + 1
assert false == (d_int < d_int - 1)
assert d_int != d_short + 1
assert d_int == d_double + 0
assert d_int == d_short
assert false == (d_int != d_short)
assert 2.Equals(d_short & (d_byte + 4))
assert 6L.Equals(d_short | (d_long + 4))
try:
	assert d_double | d_int
	print "uh-oh"
except e:
	pass

s as duck = "x"
assert s + s + s == "xxx"
