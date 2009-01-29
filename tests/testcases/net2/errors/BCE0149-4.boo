"""
BCE0149-4.boo(8,12): BCE0149: The type 'string' must derive from 'Test[of T]' in order to substitute the generic parameter 'T' in 'Test[of T]'.
"""

class Test [of T(Test[of T])]:
	pass

assert Test[of string]() != null

