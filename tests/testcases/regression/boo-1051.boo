"""
42
"""

interface Interface1:
	Foo as string: 
		get:
			pass
		set:
			pass

interface Interface2:
	Foo as int:
		get:
			pass
		set:
			pass

class Class (Interface1, Interface2):
	Interface1.Foo:
		get:
			return ""
		set:
			pass

	Foo:
		get:
			return 40
		set:
			pass

print Class().Foo + 2
