"""
foo!
bar!
foo!foo!
bar!bar!
"""
class Foo:

	public bar = { msg | print(msg) }
	
f = Foo()
f.bar("foo!")
f.bar("bar!")
f.bar = { msg | print(msg.ToString()*2) }
f.bar("foo!")
f.bar("bar!")
	
	
