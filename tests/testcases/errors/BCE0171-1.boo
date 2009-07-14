"""
BCE0171-1.boo(28,23): BCE0171: Constant value `-1' cannot be converted to a `byte'.
BCE0171-1.boo(28,27): BCE0171: Constant value `256' cannot be converted to a `byte'.
BCE0171-1.boo(29,31): BCE0171: Constant value `-32769' cannot be converted to a `short'.
BCE0171-1.boo(29,45): BCE0171: Constant value `32768' cannot be converted to a `short'.
BCE0171-1.boo(30,28): BCE0171: Constant value `-1' cannot be converted to a `ushort'.
BCE0171-1.boo(30,32): BCE0171: Constant value `65536' cannot be converted to a `ushort'.
BCE0171-1.boo(31,52): BCE0171: Constant value `-2147483649' cannot be converted to a `int'.
BCE0171-1.boo(31,70): BCE0171: Constant value `2147483648L' cannot be converted to a `int'.
BCE0171-1.boo(32,31): BCE0171: Constant value `-1' cannot be converted to a `uint'.
BCE0171-1.boo(32,35): BCE0171: Constant value `4294967296L' cannot be converted to a `uint'.
BCE0171-1.boo(35,5): BCE0171: Constant value `-1' cannot be converted to a `byte'.
BCE0171-1.boo(36,17): BCE0171: Constant value `2147483648L' cannot be converted to a `byte'.
BCE0171-1.boo(40,10): BCE0171: Constant value `-1' cannot be converted to a `uint'.
BCE0171-1.boo(41,22): BCE0171: Constant value `6442450941L' cannot be converted to a `uint'.
BCE0171-1.boo(45,5): BCE0171: Constant value `256' cannot be converted to a `byte'.
BCE0171-1.boo(46,17): BCE0171: Constant value `2147483648L' cannot be converted to a `byte'.
"""


static class Test:
	X as uint:
		set: pass

def Foo(b as byte):
	pass

b = (of byte: 0, 255, -1, 256)
s = (of short: -32768, 32767, -32769, 32767 + 1)
us = (of ushort: 0, 65535, -1, 65536)
i = (of int: -2147483648, 2147483647, int.MinValue - 1, int.MaxValue + 1)
ui = (of uint: 0, 4294967295, -1, 4294967296)

Foo(255L)
Foo(-1) #!
Foo(int.MaxValue+1) #!

Test.X = 42424242L
Test.X = int.MaxValue+1
Test.X = -1 #!
Test.X = int.MaxValue*3#!

x as byte
x = 255L
x = 256 #!
x = int.MaxValue+1 #!

