"""
BCW0013-1.boo(5,16): BCW0013: WARNING: 'constructor' on static type 'Test' is redundantly marked static. All members of static types are automatically assumed to be static.
"""
static class Test:
	static def constructor():
		print "Static Constructor"
