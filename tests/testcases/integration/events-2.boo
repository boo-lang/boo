"""
before subscribers
nothing printed
clicked!
clicked!
clicked again!
clicked!
"""
import System
import NUnit.Framework

class Button:
	event Click as EventHandler
	
	def RaiseClick():
		Click(self, EventArgs.Empty)
	
def click(sender, args as EventArgs):
	print("clicked again!")

b = Button()

print("before subscribers")
b.RaiseClick()
print("nothing printed")

b.Click += def (sender, args):
	print("clicked!")
	Assert.AreSame(sender, b)
	Assert.AreSame(EventArgs.Empty, args)
	
b.RaiseClick()

b.Click += click
b.RaiseClick()
	
b.Click -= click
b.RaiseClick()
