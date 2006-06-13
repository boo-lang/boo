"""
clicked!
clicked!

"""
import BooCompiler.Tests

def clicked(sender, args as System.EventArgs):
	print("clicked!")

c = Clickable(Click: clicked)
c.RaiseClick()
c.RaiseClick()
