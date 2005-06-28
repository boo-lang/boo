"""
public class ClassWithField(System.Object):

	public def constructor():
		super()
		self.___initializer()

	protected _name as System.String

	def ___initializer() as System.Void:
		self._name = ''
"""

class ClassWithField:
	def constructor():
		pass

	_name = ""
