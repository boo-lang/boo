"""
CannotConvertFooToInt.boo(11,11): BCE0022: Cannot convert 'Foo' to 'int'.
CannotConvertFooToInt.boo(12,16): BCE0022: Cannot convert 'FinalFoo' to 'int'.
"""
class Foo:
	pass
	
final class FinalFoo(Foo):
	pass

a = Foo() cast int
b = FinalFoo() cast int
