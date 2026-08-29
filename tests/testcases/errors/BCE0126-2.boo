"""
BCE0126-2.boo(8,15): BCE0126: It is not possible to evaluate an expression of type 'void'.
"""
def foo() as void:
	pass

def bar():	
	return foo()
