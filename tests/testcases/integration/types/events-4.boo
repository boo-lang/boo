"""
before
no handlers
after
before
got it!
after
"""
import System

class Observable:

	event Changed as EventHandler
	
	def RaiseChanged():
		print("before")
		if Changed is null:
			print("no handlers")
		else:
			# the event reference can be treated
			# as a delegate
			Changed.Invoke(self, EventArgs.Empty)
		print("after")
			
			
o = Observable()
o.RaiseChanged()
o.Changed += { print("got it!") }
o.RaiseChanged()
