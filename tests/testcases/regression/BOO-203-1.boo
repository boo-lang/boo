"""
MissingMethodException
10
"""
a as duck = 4
b = a + 3 + 3
try:
	b.Foo()
	raise "Foo is not a method of int"
except x as System.MissingMethodException:
	print "MissingMethodException"
print b
