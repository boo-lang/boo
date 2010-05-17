"""
class Foo:
	pass

class Bar:
	pass

macro1 foo
macro2 bar:
	a as int
assert true
assert false
Foo()
"""
macro1 foo
class Foo:
end
	
macro2 bar:
	a as int
end
class Bar:
end
	
assert true
assert false
Foo()
