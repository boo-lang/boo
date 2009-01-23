"""
BCW0023-1.boo(14,9): BCW0023: WARNING: This method could return default value implicitly.
BCW0023-1.boo(29,9): BCW0023: WARNING: This method could return default value implicitly.
BCW0023-1.boo(44,9): BCW0023: WARNING: This method could return default value implicitly.
"""
macro enableBCW0023:
	Context.Parameters.EnableWarning("BCW0023")
enableBCW0023

class Foo:
	_x = 1

	Bar as int:
		get:#!
			if _x < 0:
				return _x
			#implicit return 0 => warning

	Baz as int:
		get:
			raise "foo"

	def Ok() as bool:
		if _x > 0:
			return true
		else:
			return false

	def Bad() as bool: #!
		if _x > 0:
			pass
		else:
			return false

	def TryGood() as bool:
		try:
			print ""
			return false
		except:
			return true
		ensure:
			pass

	def TryBad() as bool: #!
		try:
			print ""
			return false
		except as System.ArgumentException:
			return true
		except:
			pass

macro ignore:
	assert Context.Parameters is not null

macro xxx:
	return [| print "xxx" |]

macro yyy:
	yield

xxx
yyy

