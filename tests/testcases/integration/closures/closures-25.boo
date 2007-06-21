"""
: foo!
: bar!
spam!
before
end.
"""
class Foo:

	public bar = def (msg):
		print("${prefix}${msg}")
	
	prefix = ": "
	
f = Foo()
f.bar("foo!")
f.bar("bar!")

f.bar = def (msg):
	print(msg)
	
f.bar("spam!")

f.bar = def (msg):
	pass
	
print("before")
f.bar("eggs!")
print("end.")
