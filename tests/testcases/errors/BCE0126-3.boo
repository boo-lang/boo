"""
BCE0126-3.boo(8,14): BCE0126: It is not possible to evaluate an expression of type 'void'.
"""
def foo() as void:
	pass

def bar():	
	yield foo()
