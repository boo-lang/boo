"""
public class Foo(System.Object):

	protected _first as System.Int32

	protected _second as System.Int32

	public def constructor():
		super()
		self.___initializer()

	public def constructor(bar as System.Object):
		super()
		self.___initializer()

	def ___initializer() as System.Void:
		self._first = 14
		self._second = (self._first * 2)
"""
class Foo:
	_first = 14
	_second = _first*2
	
	def constructor():
		pass
		
	def constructor(bar):
		pass
