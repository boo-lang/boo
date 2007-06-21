"""
BCE0120-3.boo(24,7): BCE0120: 'NS2.Foo.get_C' is inaccessible due to its protection level.
BCE0120-3.boo(25,7): BCE0120: 'NS2.Foo.get_D' is inaccessible due to its protection level.
BCE0120-3.boo(26,3): BCE0120: 'NS2.Foo.D' is inaccessible due to its protection level.
"""
namespace NS2

class Foo:
	protected C:
		get:
			return "a string"
			
	private D:
		get:
			return 1
		set:
			pass
	
class Bar(Foo):
	def constructor():
		print self.C

f = Foo()
print f.C
print f.D
f.D = 2

