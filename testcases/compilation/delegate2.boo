"""
handler - clicked!
handler - clicked!

"""
import Boo.Tests.Ast.Compiler from Boo.Tests

class Handler:
	
	tag
	
	def constructor(tag):
		self.tag = tag
		
	def clicked(sender, args as System.EventArgs):
		print("${tag} - clicked!")

c = Clickable()
c.Click += Handler("handler").clicked
c.RaiseClick()
c.RaiseClick()
