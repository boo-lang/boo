"""
Foo = 1
Foo = -1
"""
import BooCompiler.Tests

def testByteEnum():
	a = ByteEnum.Foo
	assert ByteEnum is a.GetType()
	b = cast(byte, ByteEnum.Foo)
	assert 1 == b

	print "${a} = ${b}"
	
def testSByteEnum():
	a = SByteEnum.Foo
	assert SByteEnum is a.GetType()
	b = cast(sbyte, SByteEnum.Foo)
	assert -1 == b
	
	print "${a} = ${b}"
	
testByteEnum()
testSByteEnum()
