"""
field
"""
class fld:
	def constructor():
		print "fld.constructor"

class C:
	public fld as callable = { print "field" }
	
	def doit():
		fld()

C().doit()
