"""
clicked!
clicked!

"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

def clicked(sender, args as System.EventArgs):
	print("clicked!")

c = Clickable()
c.Click += clicked
c.RaiseClick()
c.RaiseClick()
