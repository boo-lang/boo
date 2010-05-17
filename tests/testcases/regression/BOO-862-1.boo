"""
True
True
"""
def test() as single:
	return 0.0

yeah = not (test() or test())
print(yeah)
yeah = not (test() and test())
print(yeah)
