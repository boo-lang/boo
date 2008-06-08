"""
Foo
IFoo1.Foo
"""

interface IFoo1:
	Foo as string: 
		get; 

interface IFoo2:
	Foo as string:
		get:
			pass

class Bar(IFoo1, IFoo2):
	IFoo1.Foo:
		get:
			return "IFoo1.Foo"

	Foo:
		get:
			return "Foo"
bar = Bar()
print bar.Foo

ifoo1 as IFoo1 = bar
print ifoo1.Foo
