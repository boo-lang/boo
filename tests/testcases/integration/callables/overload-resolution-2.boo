"""
bar(int, int, int)
bar(single, single, single)
bar(single, single, single)
bar(single, single, single)
"""
class Foo:
	def bar(a1 as int, a2 as int, a3 as int):
		print "bar(int, int, int)"
		
	def bar(a1 as single, a2 as single, a3 as single):
		print "bar(single, single, single)"
	
i as int
s as single

foo = Foo()
foo.bar(i, i, i)
foo.bar(i, i, s)
foo.bar(i, s, s)
foo.bar(s, s, s)
