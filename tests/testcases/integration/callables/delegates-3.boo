"""
handler - clicked!
handler - clicked!

"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

class Handler:
	
	tag = null
	
	def constructor(tag):
		self.tag = tag
		
	def clicked(sender, args as System.EventArgs):
		print("${tag} - clicked!")

c = Clickable()
c.Click += Handler("handler").clicked
c.RaiseClick()
c.RaiseClick()
