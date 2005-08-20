"""
def foo():
	pass

a = ('' isa typeof(string))
b = (foo isa typeof(callable))
c = ((1, 2, 3) isa typeof((int)))
"""
def foo():
	pass
	
a = '' isa string
b = foo isa callable
c = (1, 2, 3) isa (int)


