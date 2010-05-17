"""
42
"""

class Base[of T]:
	public abstract def Method(arg as T) as T:
		pass

class Derived[of U] (Base[of U]):
	def Method(arg as U):
		return arg

instance as Base[of int] = Derived[of int]()
print instance.Method(42)
