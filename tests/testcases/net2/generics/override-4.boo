"""
Overridden!
spam
"""

class Base[of T(class)]:
	public virtual def Method(arg as T) as T:
		return arg

class Derived[of U(class)] (Base[of U]):
	def Method(arg as U):
		print "Overridden!"
		return super.Method(arg)

instance as Base[of string] = Derived[of string]()
print instance.Method("spam")
