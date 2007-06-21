"""
doing it
3
doing it
3
"""
class A:
	static public Instance = B()

class B:
	def doit():
		print 'doing it'
		return 3

print(cast(duck, A).Instance.doit())
print((A as duck).Instance.doit())
