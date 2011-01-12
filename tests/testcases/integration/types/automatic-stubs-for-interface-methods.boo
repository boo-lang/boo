import System

interface A:
	def Test(o)
	
interface B(A):
	pass
	
class Foo(B):
	pass
	
assert not typeof(Foo).IsAbstract

method = typeof(Foo).GetMethod("Test")
assert method is not null
assert not method.IsAbstract
assert method.ReturnType is void
assert 1 == len(method.GetParameters())
assert method.GetParameters()[0].ParameterType is object

try:
	Foo().Test(null)
	assert false
except x as NotImplementedException:
	pass
