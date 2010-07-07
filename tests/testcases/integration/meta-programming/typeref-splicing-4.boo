"""
(a as Foo)
(a as IFoo)
(a as SFoo)
(a as Foo[of string])
(a as IFoo[of string])
(a as SFoo[of string])
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
	code = [| a as $e |]
	print code.ToCodeString()

types = (Foo, IFoo, SFoo, Foo of string, IFoo of string, SFoo of string)
for type in types:
	test type


