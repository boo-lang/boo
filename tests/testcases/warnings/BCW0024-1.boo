"""
BCW0024-1.boo(17,5): BCW0024: WARNING: Visible property does not declare return type explicitely.
BCW0024-1.boo(28,5): BCW0024: WARNING: Visible property does not declare return type explicitely.
BCW0024-1.boo(38,19): BCW0024: WARNING: Visible method does not declare return type explicitely.
BCW0024-1.boo(44,9): BCW0024: WARNING: Visible method does not declare 's' argument type explicitely.
BCW0024-1.boo(50,9): BCW0024: WARNING: Visible method does not declare return type explicitely.
BCW0024-1.boo(57,13): BCW0024: WARNING: Visible method does not declare return type explicitely.
BCW0024-1.boo(78,9): BCW0024: WARNING: Visible constructor does not declare 'y' argument type explicitely.
"""
macro enableBCW0024disableBCW0014:
	Context.Parameters.EnableWarning("BCW0024")
	Context.Parameters.DisableWarning("BCW0014") #unused privates
enableBCW0024disableBCW0014
interface IFoo:
	Good as int:
		get
	Bad: #!
		get

class Foo:
	Good as bool:
		get:
			return true
	private NotSoGood:
		get:
			return true

	Bad: #!
		get:
			return true

	protected def GoodMeth() as bool:
		return true

	internal def NotSoGoodMeth():
		return false

	protected def BadMeth(): #!
		return false

	def GoodMethArg(s as string) as bool:
		pass

	def BadMethArg(s) as bool: #!
		pass

	def GoodVoid() as void:
		pass

	def BadVoid(): #!
		pass

	private def OkVoid():
		pass

	public class Nested:
		def ImplicitBool(): #!
			return true


private class PrivateFoo:
	public NonVisiblePublicProperty:
		get:
			return true

	public def NonVisiblePublicMethod():
		return true

	public class NestedPublic:
		public def NonVisibleNestedPublicMethod():
			return false


class Ctors:
	private def constructor(x,y,z):
		pass

	def constructor(y): #!
		pass

