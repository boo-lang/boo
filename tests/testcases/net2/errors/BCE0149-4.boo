#ignore internal constraint to inherit self is silently dropped currently
"""
BCE00149-4.boo...
"""

class Test [of T(Test[of T])]:
	pass

assert Test[of string]() != null

