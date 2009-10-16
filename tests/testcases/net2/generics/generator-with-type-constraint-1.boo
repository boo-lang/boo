#ignore generic generators not supported yet
"""
D
"""
class B:
	pass
    
class D(B):
	pass

class C[of T]:
	
	public items as List

	def Select[of U(T)]() as U*:
		for item in items:
			yield item if item isa U
			
for d in C[of B](items: [B(), D()]).Select[of D]():
	print d
