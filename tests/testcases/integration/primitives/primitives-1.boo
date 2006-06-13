import NUnit.Framework

def same(expected, actual):
	Assert.AreSame(expected, actual)
	
same(System.Byte, byte)
same(System.Boolean, bool)
same(System.Int16, short)
same(System.UInt16, ushort)
same(System.Int32, int)
same(System.UInt32, uint)
same(System.Int64, long)
same(System.UInt64, ulong)
same(System.Single, single)
same(System.Double, double)
same(System.String, string)
same(System.Object, object)
same(System.Void, void)
same(System.DateTime, date)
