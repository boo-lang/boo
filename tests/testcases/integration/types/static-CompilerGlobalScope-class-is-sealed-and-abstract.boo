"""
"""
import System.Reflection

[System.Runtime.CompilerServices.CompilerGlobalScope]
static class Foo:
	pass
	
type = Foo
assert type.IsSealed
assert type.IsAbstract
assert type.Attributes & TypeAttributes.Sealed
assert type.Attributes & TypeAttributes.Abstract


