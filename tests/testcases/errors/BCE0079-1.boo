"""
BCE0079-1.boo(8,11): BCE0079: __addressof__ built-in function can only be used in delegate constructors.
"""
class Clerk:
	static def punch():
		pass
		
address = __addressof__(Clerk.punch)

