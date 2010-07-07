"""
a = Foo
a = IFoo
a = SFoo
a = Foo[of Bar]
a = IFoo[of Bar]
a = SFoo[of Bar]
"""
class Bar:
	pass
	
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

types = (Foo, IFoo, SFoo, Foo of Bar, IFoo of Bar, SFoo of Bar)
for type in types:
	test type


