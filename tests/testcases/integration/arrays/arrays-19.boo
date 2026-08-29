"""
Foo.Bar.Person[][]
"""
namespace Foo.Bar

class Person:
	pass

items = ((Person(), Person()), (Person(),))
print(items.GetType())
