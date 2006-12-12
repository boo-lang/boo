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
end

interface IBar(IFoo):
end

class Person(IFoo):
end

class Customer(Person, IBar, IFoo):
end

