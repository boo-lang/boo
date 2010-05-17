"""
interface IFoo:
	pass

interface IBar(IFoo):
	pass

class Person(IFoo):
	pass

class Customer(Person, IBar, IFoo):
	pass

"""
interface IFoo:
	pass

interface IBar(IFoo):
	pass
	
class Person(IFoo):
	pass
	
class Customer(Person, IBar, IFoo):
	pass
