import NUnit.Framework

callable Function(item) as object

callable FunctionGenerator(item) as Function

class Adder:

	_amount = 0
	
	def constructor(amount):
		_amount = amount
		
	def Add(value as int):
		return value+_amount
		
	static def Create(amount):
		return Adder(amount).Add
		
generator as FunctionGenerator = Adder.Create
plus3 = generator(3)
Assert.AreSame(Function, plus3.GetType())
Assert.AreEqual(5, plus3(2))
