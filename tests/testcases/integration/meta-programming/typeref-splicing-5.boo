"""
(a = Foo)
(a = IFoo)
(a = SFoo)
(a = Foo[of string])
(a = IFoo[of string])
(a = SFoo[of string])
"""
class Foo:
	pass
	
interface IFoo:
	pass
	
struct SFoo:
	value as int
	
class Foo[of T]:
	pass
	
class IFoo[of T]:
	pass
	
struct SFoo[of T]:
	value as int
	
def test(e as System.Type):
	code = [| a = $e |]
	print code.ToCodeString()

types = (Foo, IFoo, SFoo, Foo of string, IFoo of string, SFoo of string)
for type in types:
	test type


