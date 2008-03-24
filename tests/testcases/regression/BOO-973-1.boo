"""
OnExternalEvent
OnInternalEvent
"""

import System

callable MyEvent(sender as object, e as EventArgs)

class GenericFoo[of T]:
	event AnExternalEvent as EventHandler
	event AnInternalEvent as MyEvent

	def TestEvents():
		AnExternalEvent(self, EventArgs.Empty)
		AnInternalEvent(self, EventArgs.Empty)
	
def OnExternalEvent(sender as object, e as EventArgs):
	print "OnExternalEvent"

def OnInternalEvent(sender as object, e as EventArgs):
	print "OnInternalEvent"

foo = GenericFoo[of string]()
foo.AnExternalEvent += OnExternalEvent
foo.AnInternalEvent += OnInternalEvent
foo.TestEvents()