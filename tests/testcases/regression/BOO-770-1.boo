"""
-1
"""
class Bug:
	[property(Id)] _id = 0
	
	static def Load():
		return Bug(Id: -1)
		
print Bug.Load().Id
