"""
class Maybe[of T]:
	pass

class Some[of T](Maybe[of T]):

	public value as T

class None[of T](Maybe[of T]):
	pass
"""
class Maybe[T]:
	pass
	
class Some[T](Maybe[T]):
	public value as T
	
class None[T](Maybe[T]):
	pass
