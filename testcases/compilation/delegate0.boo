"""
clicked!
clicked!

"""
using Boo.Tests.Ast.Compilation

def clicked(sender, args as System.EventArgs):
	print("clicked!")

c = Clickable(Click: clicked)
c.RaiseClick()
c.RaiseClick()
