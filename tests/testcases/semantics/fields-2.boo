"""
public class ClassWithField(object):

	protected _name as string

	public def constructor():
		super()
		self.$initializer$()

	def $initializer$() as void:
		self._name = ''
"""

class ClassWithField:
	_name = ""
	
