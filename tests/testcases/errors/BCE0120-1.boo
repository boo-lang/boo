"""
BCE0120-1.boo(16,20): BCE0120: 'NS.Foo._baz' is inaccessible due to its protection level.
BCE0120-1.boo(20,9): BCE0120: 'NS.Foo._bar' is inaccessible due to its protection level.
BCE0120-1.boo(21,9): BCE0120: 'NS.Foo._baz' is inaccessible due to its protection level.
"""
namespace NS

class Foo:
	protected _bar
	private _baz
	public bang
	
class Bar(Foo):
	def constructor():
		print self._bar
		print self._baz
		print self.bang
	
f = Foo()
print f._bar
print f._baz
print f.bang
	
