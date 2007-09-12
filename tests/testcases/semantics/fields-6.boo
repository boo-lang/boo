"""
public class ClassWithField(object):

	public def constructor():
		super()
		self.$initializer$()

	protected _name as string

	def $initializer$() as void:
		self._name = ''
"""

class ClassWithField:
	def constructor():
		super()

	_name = ""
