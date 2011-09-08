"""
"""
import System.Reflection

static class Foo:
	pass
	
type = Foo
assert type.IsSealed
assert type.IsAbstract
assert type.Attributes & TypeAttributes.Sealed
assert type.Attributes & TypeAttributes.Abstract
assert not type.IsSerializable


