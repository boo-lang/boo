"""
clicked!
clicked!

"""
using Boo.Tests.Ast.Compilation

def clicked(sender, args as System.EventArgs):
	print("clicked!")

c = Clickable()
c.Click += clicked
c.RaiseClick()
c.RaiseClick()
