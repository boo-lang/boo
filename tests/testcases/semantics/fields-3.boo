"""
public class Foo(object):

	protected _first as int

	protected _second as int

	public def constructor():
		super()
		self.$initializer$()

	public def constructor(bar as object):
		super()
		self.$initializer$()

	def $initializer$() as void:
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
