"""
BCE0189-1.boo(6,11): BCE0189: 'x' is passed by reference, so it cannot declare a default.
"""
# A ref parameter always reads its argument from the call, so a default has
# nothing to stand in for; C# rejects the same shape as CS1741.
def f(ref x as int = 10):
	pass
