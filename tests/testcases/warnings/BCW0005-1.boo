"""
BCW0005-1.boo(21,8): BCW0005: WARNING: Unsubscribing from event 'Foo.Bang' with an adapted method reference. Either change the signature of the method to 'callable(object, System.EventArgs) as void' or use a cached reference of the correct type.
BCW0005-1.boo(22,9): BCW0005: WARNING: Unsubscribing from event 'Foo.Crash' with an adapted method reference. Either change the signature of the method to 'callable(object) as void' or use a cached reference of the correct type.
"""
import System

class Foo:
	event Bang as EventHandler
	event Crash as callable(object)
	
def spam():
	pass
	
def eggs(i as int):
	pass
	
def correct(sender, args as EventArgs):
	pass

f = Foo()
f.Bang -= spam
f.Crash -= eggs
f.Bang -= correct
