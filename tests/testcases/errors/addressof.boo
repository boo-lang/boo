"""
addressof.boo(8,11): BCE0079: __addressof__ builtin function can only be used in delegate constructors.
"""
class Clerk:
	def punch():
		pass
		
address = __addressof__(Clerk.punch)

