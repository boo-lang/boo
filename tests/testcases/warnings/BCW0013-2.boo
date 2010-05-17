"""
BCW0013-2.boo(6,12): BCW0013: WARNING: 'eigen' on static type 'Test' is redundantly marked static. All members of static types are automatically assumed to be static.
BCW0013-2.boo(7,16): BCW0013: WARNING: 'DoTheTest' on static type 'Test' is redundantly marked static. All members of static types are automatically assumed to be static.
"""
static class Test:
	static eigen = 42
	static def DoTheTest():
		print eigen
