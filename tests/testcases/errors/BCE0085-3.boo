"""
BCE0085-3.boo(8,5): BCE0085: Cannot create instance of abstract class 'Foo'.
"""
class Foo:
	abstract def Bar():
		pass
	
f = Foo()
