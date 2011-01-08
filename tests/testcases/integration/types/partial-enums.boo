"""
Foo = 0
Bar = 1
"""
partial enum E:
	Foo
	
partial enum E:
	Bar

for value in System.Enum.GetValues(E):
	print value, "=", value cast int
	

