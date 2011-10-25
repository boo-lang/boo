"""
before subscribers
nothing printed
clicked!
clicked!
clicked again!
clicked!
"""
import System

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
	assert sender is b
	assert EventArgs.Empty is args
	
b.RaiseClick()

b.Click += click
b.RaiseClick()
	
b.Click -= click
b.RaiseClick()
