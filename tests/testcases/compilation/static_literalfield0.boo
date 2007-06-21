"""
255
System.Byte
127
System.SByte
32767
System.Int16
65535
System.UInt16
2147483647
System.Int32
4294967295
System.UInt32
9223372036854775807
System.Int64
18446744073709551615
System.UInt64
3.402823E+38
System.Single
1.79769313486232E+308
System.Double
System.Char
"""
import System.Globalization

print byte.MaxValue
print byte.MaxValue.GetType()
print sbyte.MaxValue
print sbyte.MaxValue.GetType()
print short.MaxValue
print short.MaxValue.GetType()
print ushort.MaxValue
print ushort.MaxValue.GetType()
print int.MaxValue
print int.MaxValue.GetType()
print uint.MaxValue
print uint.MaxValue.GetType()
print long.MaxValue
print long.MaxValue.GetType()
print ulong.MaxValue
print ulong.MaxValue.GetType()
print single.MaxValue.ToString(CultureInfo.InvariantCulture)
print single.MaxValue.GetType()
print double.MaxValue.ToString(CultureInfo.InvariantCulture)
print double.MaxValue.GetType()
print char.MaxValue.GetType()
