"""
BCE0020-2.boo(11,19): BCE0020: An instance of type 'foo' is required to access non static member 'types'.
"""
class foo:
	types = ['hello', 'world!']
	def fu():
		b = self.bar()
		b.fark()
	class bar:
		def fark():
			print types

f = foo()
f.fu()

