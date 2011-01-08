"""
Foo
Bar
"""
partial interface I:
	def Foo()
	
partial interface I:
	def Bar()

for member in typeof(I).GetMembers():
	print member.Name
	

