"""
let's see whose quack fu is better!
whoooooyah!
"""
import System

class Expando(IQuackFu):

	_attributes = {}
	
	def QuackSet(name as string, value):
		_attributes[name] = value
		return value

	def QuackGet(name as string):		
		return _attributes[name]
		
	def QuackInvoke(name as string, args as (object)) as object:
		(_attributes[name] as callable).Call(args)

e as duck = Expando()

e.Speak = def (item):
	print item	
e.Speak("let's see whose quack fu is better!")

e.Speak = { print "whoooooyah!" }
e.Speak()


