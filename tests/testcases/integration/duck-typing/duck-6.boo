"""
let's see whose quack fu is better!
whoooooyah!
Speak not found!
"""
import System

class Expando(IQuackFu):

	_attributes = {}
	
	def QuackSet(name as string, parameters as (object), value):
		assert parameters is null
		_attributes[name] = value
		return value

	def QuackGet(name as string, parameters as (object)):
		assert parameters is null
		return _attributes[name]
		
	def QuackInvoke(name as string, args as (object)) as object:
		if name == "op_Subtraction":
			_attributes.Remove(args[1])
		else:
			(_attributes[name] as callable).Call(args)

e as duck = Expando()

e.Speak = def (item):
	print item	
e.Speak("let's see whose quack fu is better!")

e.Speak = { print "whoooooyah!" }
e.Speak()

e -= "Speak"
try:
	e.Speak()
except:
	print "Speak not found!"
