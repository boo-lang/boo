"""
BCE0120-4.boo(14,6): BCE0120: 'NS.Foo.constructor' is inaccessible due to its protection level.
BCE0120-4.boo(15,9): BCE0120: 'NS.Foo.constructor' is inaccessible due to its protection level.
"""
namespace NS

class Foo:
	private def constructor():
		pass
		
	static def NewInstance():
		return Foo()
	
f1 = Foo()
f2 = NS.Foo()
	
