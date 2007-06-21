"""
fld.constructor
"""
class fld:
	def constructor():
		print "fld.constructor"

class C:
	public fld as int
	
	def doit():
		# field is not callable, should look for a callable
		# and find the type defined above instead
		fld()

C().doit()
