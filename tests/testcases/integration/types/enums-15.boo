"""
'Foo' flag is set
'Baz' flag is set
"""
[System.Flags]
enum FooBarFlags:
	None = 0
	Foo = 1
	Bar = 2
	Baz = 4

flags = FooBarFlags.Foo | FooBarFlags.Baz
print "'Foo' flag is set" if FooBarFlags.Foo & flags
print "'Bar' flag is set" if FooBarFlags.Bar & flags	
print "'Baz' flag is set" if FooBarFlags.Baz & flags
	
