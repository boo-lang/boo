"""
public class Foo(object):

	protected _first as int

	protected _second as int

	public def constructor():
		super()
		if self.\$initialized__Foo\$:
			goto ___initialized___
		self._first = 14
		self._second = (self._first * 2)
		self.\$initialized__Foo\$ = true
		:___initialized___

	public def constructor(bar as object):
		super()
		if self.\$initialized__Foo\$:
			goto ___initialized___
		self._first = 14
		self._second = (self._first * 2)
		self.\$initialized__Foo\$ = true
		:___initialized___

	private \$initialized__Foo\$ as bool
"""
class Foo:
	_first = 14
	_second = _first*2
	
	def constructor():
		pass
		
	def constructor(bar):
		pass
