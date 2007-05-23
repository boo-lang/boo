"""
double: 1.5
int: 1
double, *: 1.5 1 2 3
int, *: 1 1 2 3
"""
class Foo:
	def bar(i as int):
		print "int:", i
	def bar(d as double):
		print "double:", d
	def bar(i as int, *args):
		print "int, *:", i, join(args)
	def bar(d as double, *args):
		print "double, *:", d, join(args)
		
d as duck = Foo()
d.bar(1.5)
d.bar(1)
d.bar(1.5, 1, 2, 3)
d.bar(1, 1, 2, 3)

