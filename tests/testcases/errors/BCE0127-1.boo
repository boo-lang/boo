"""
BCE0127-1.boo(9,6): BCE0127: A ref or out argument must be an lvalue: '10'
BCE0127-1.boo(9,5): BCE0017: The best overload for the method 'BCE0127-1Module.doit(int)' is not compatible with the argument list '(int)'.
"""

def doit(ref y as int):
	y = 4

doit(10)

