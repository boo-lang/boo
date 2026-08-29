"""
2, 4, 6
"""
struct Foo:
	value as int
	
	override def ToString():
		return value.ToString()
	
foos = array(Foo, 3)
i = 0
for foo in foos:
	foo.value += 2*(++i)
	
print join(foos, ', ')
