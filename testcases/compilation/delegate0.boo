"""
clicked!
clicked!

"""
import Boo.Lang.Compiler.Tests from Boo.Lang.Compiler.Tests

def clicked(sender, args as System.EventArgs):
	print("clicked!")

c = Clickable(Click: clicked)
c.RaiseClick()
c.RaiseClick()
