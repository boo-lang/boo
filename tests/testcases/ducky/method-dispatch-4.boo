"""
double: 1.5
int: 1
"""
class Foo:
	def bar(i as int):
		print "int:", i
	def bar(d as double):
		print "double:", d
		
d as duck = Foo()
d.bar(1.5)
d.bar(1)

