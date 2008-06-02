"""
BCW0017-1.boo(23,19): BCW0017: WARNING: New protected method 'Bar' declared in sealed class 'B'.
BCW0017-1.boo(30,15): BCW0017: WARNING: New protected property 'Y' declared in sealed class 'B'.
BCW0017-1.boo(36,5): BCW0014: WARNING: Private field 'B._y' is never used.
BCW0017-1.boo(38,15): BCW0017: WARNING: New protected field '_z' declared in sealed class 'B'.
"""

class A:
	protected def Foo():
		pass

	X:
		virtual protected set:
			_x = value

	_x = 0


final class B(A):
	protected def Foo(): #do not warn since override
		pass

	protected def Bar(): #warn
		pass

	X:
		protected set:
			_x = 0

	protected Y:
		get:
			return 0
		protected set:
			pass

	_y = 0 #automatically make it private

	protected _z = 0 #do not make it private

