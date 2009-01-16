"""
BCW0011-11.boo(33,12): BCW0011: WARNING: Type 'Foo' does not provide an implementation for 'IFoo[of string].Bla()()', a stub has been created.
BCW0011-11.boo(33,12): BCW0011: WARNING: Type 'Foo' does not provide an implementation for 'IFoo[of string].Bar', a stub has been created.
BCW0011-11.boo(33,12): BCW0011: WARNING: Type 'Foo' does not provide an implementation for 'IFoo[of string].Baz', a stub has been created.
BCW0011-11.boo(33,12): BCW0011: WARNING: Type 'Foo' does not provide an implementation for 'IFoo[of string].Half', a stub has been created.
BCW0011-11.boo(33,29): BCW0011: WARNING: Type 'Foo' does not provide an implementation for 'AFoo.ABar', a stub has been created.
BCW0011-11.boo(33,29): BCW0011: WARNING: Type 'Foo' does not provide an implementation for 'AFoo.ABaz', a stub has been created.
"""

interface IFoo[of T]:
	def Bla()

	Bar as int:
		get

	Baz as T:
		get
		set

	Half as int:
		get
		set

abstract class AFoo:
	abstract ABar:
		get:
			pass

	ABaz:
		abstract get:
			pass

class Foo (IFoo[of string], AFoo):
	Half as int:
		get:
			return 0
		#set is not implemented


foo = Foo()
assert foo.Half == 0
foo.Bla()
assert foo.Bar
assert foo.Baz
assert foo.ABar
assert foo.ABaz

