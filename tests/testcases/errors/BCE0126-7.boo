"""
BCE0126-7.boo(7,16): BCE0126: It is not possible to evaluate an expression of type 'void'.
"""
def foo() as void:
	pass
	
h = {"foo": foo()}
print h
