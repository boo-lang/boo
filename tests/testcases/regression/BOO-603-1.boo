"""
secret
"""
class Person:
	[Property(Name)]
	_name as string

	[Property(Labels)]
	_labels as Hash

p1 as duck = Person(Name:"Happy", Labels:{1:'secret', 2:''})
print p1.Labels[1]
