"""
BCW0020-1.boo(13,11): BCW0020: WARNING: Assignment made to same expression. Did you mean to assign to something else?
BCW0020-1.boo(15,16): BCW0020: WARNING: Assignment made to same expression. Did you mean to assign to something else?
BCW0020-1.boo(20,3): BCW0020: WARNING: Assignment made to same expression. Did you mean to assign to something else?
BCW0020-1.boo(24,7): BCW0020: WARNING: Assignment made to same expression. Did you mean to assign to something else?
BCW0020-1.boo(28,7): BCW0020: WARNING: Assignment made to same expression. Did you mean to assign to something else?
"""
class Foo:
	public x as int
	public static y = 0

	def Bad(x as int):
		x = x #!
		self.x = x
		self.x = self.x #!

x = 1
y = 2

x = x #!
x = y
x += x

Foo.y = Foo.y #!
Foo().x = Foo().x
foo = Foo()
foo2 = Foo()
foo.x = foo.x #!
foo.x = foo2.x

