"""
BCE0164-2.boo(11,7): BCE0164: Cannot infer generic arguments for method 'BCE0164_2Module.Method[of T]()'. Provide stronger type information through arguments, or explicitly state the generic arguments.
"""

def Method(arg as int):
	pass
	
def Method[of T]():
	pass
	
Method()
