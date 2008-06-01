"""
non-generic
42
"""

class Program:
	class IntWrapper:
		def constructor(value as int):
			_value = value
		
		_value as int
		
		override def ToString():
			return _value.ToString()
	
	def Method[of T](arg as T):
		print "generic"
		return arg

	def Method(arg as IntWrapper):
		print "non-generic"
		return arg
		
print Program().Method(Program.IntWrapper(42))
