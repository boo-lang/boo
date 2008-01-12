"""
BCE0152-1.boo(5,17): BCE0152: Constructors cannot be marked virtual, abstract, or override: 'Test.constructor'.
"""
class Test:
	virtual def constructor():
		print "Virtual Constructor!!"

t = Test()
print t