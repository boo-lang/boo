"""
BCW0021-1.boo(17,25): BCW0021: WARNING: Comparison made with same expression. Did you mean to compare with something else?
BCW0021-1.boo(23,9): BCW0021: WARNING: Comparison made with same expression. Did you mean to compare with something else?
BCW0021-1.boo(24,9): BCW0021: WARNING: Comparison made with same expression. Did you mean to compare with something else?
BCW0021-1.boo(28,9): BCW0021: WARNING: Comparison made with same expression. Did you mean to compare with something else?
BCW0021-1.boo(29,9): BCW0021: WARNING: Comparison made with same expression. Did you mean to compare with something else?
BCW0021-1.boo(30,9): BCW0021: WARNING: Comparison made with same expression. Did you mean to compare with something else?
BCW0021-1.boo(31,9): BCW0021: WARNING: Comparison made with same expression. Did you mean to compare with something else?
BCW0021-1.boo(34,12): BCW0021: WARNING: Comparison made with same expression. Did you mean to compare with something else?
BCW0021-1.boo(38,12): BCW0021: WARNING: Comparison made with same expression. Did you mean to compare with something else?
"""
class Foo:
	public x as string
	public static y = 0
	def Bar(x as string):
		print 1 if self.x != x #OK
		print 2 if self is self

x = 0
y = 1
print x == y #OK

print x == x
print x != x

z = 0
a = 1
print z < z
print z > z
print z <= z
print z >= z
print z != a #OK

b = (Foo.y == Foo.y)
b = cast(bool, Foo().x != Foo().x) #OK
foo = Foo()
foo2 = Foo()
b = (foo.x is foo.x)
b = (foo2.x is foo.x) #OK

print b #use

