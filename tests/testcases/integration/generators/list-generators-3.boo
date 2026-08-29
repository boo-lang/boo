"""
[Homer, Bart]
"""
class Foo:
	[getter(Name)] _name as string
	
	def constructor(name):
		_name = name
		
all = (object(), Foo("Homer"), object(), Foo("Bart"))
names = [foo.Name for item in all if foo=(item as Foo)]
print(names)
