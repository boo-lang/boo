"""
BCW0028-2.boo(18,9): BCW0028: WARNING: Implicit downcast from `System.Collections.IEnumerable' to `Test'.
"""
import System.Collections

macro enableBCW0028:
	Parameters.Strict = true
	Parameters.EnableWarning("BCW0028") #force warning instead of error
	Parameters.DisableWarning("BCW0003") #unused locals `us' and `a'
enableBCW0028

class Test:
	pass

def Foo(x as IEnumerable):
	pass

Foo(Test()) #!

ol = ArrayList()
for s as string in ol:
	print s.StartsWith("foo") #implicit (loop intrinsic) downcast from object ok

assert null == Test() as string #explicit downcast ok

us as ushort = 1 #implicit promotion ok
a = (of ushort: 1, 2, 3)

