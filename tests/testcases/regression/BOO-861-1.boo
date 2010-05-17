"""
Hello
0
"""
class Foo:
	counter = 1
	
	def GetTicks():
		yield 0
		if --counter <= 0:
			print "Hello"
			
print join(Foo().GetTicks(), '')
