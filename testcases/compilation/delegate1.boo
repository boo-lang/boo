"""
clicked!
clicked!

"""
using Boo.Tests.Ast.Compiler

def clicked(sender, args as System.EventArgs):
	print("clicked!")

c = Clickable()
c.Click += clicked
c.RaiseClick()
c.RaiseClick()
