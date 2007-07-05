"""
BCE0142.boo(4,5): BCE0142: Cannot bind [default] attribute to value type parameter 'x' in 'method'.
"""
def method([default(0)] x as int):
	print x
method(2)
