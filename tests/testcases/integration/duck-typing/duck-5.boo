"""
QuackSet(FirstName, Homer)
QuackSet(LastName, Simpson)
QuackGet(FirstName)
QuackGet(LastName)
QuackInvoke(Speak, dough)
QuackInvoke(Walk, )
QuackInvoke(Eat, donuts!, donuts!)
QuackInvoke(Adding cream pie)
"""
import System

class Expando(IQuackFu):

	_attributes = {}
	
	def QuackSet(name as string, parameters as (object), value):
		assert parameters is null
		print "QuackSet(${name}, ${value})"
		_attributes[name] = value
		return value

	def QuackGet(name as string, parameters as (object)):
		assert parameters is null
		print "QuackGet(${name})"
		raise "attribute not found: ${name}" if name not in _attributes
		return _attributes[name]
		
	def QuackInvoke(name as string, args as (object)) as object:
		if name == "op_Addition":
			print "QuackInvoke(Adding ${args[1]})"
		else:
			print "QuackInvoke(${name}, ${join(args, ', ')})"

e as duck = Expando()
e.FirstName = "Homer"
e.LastName = "Simpson"
assert "Homer Simpson" == "${e.FirstName} ${e.LastName}"

e.Speak('dough')
e.Walk()
e.Eat('donuts!', 'donuts!')

e += "cream pie"
