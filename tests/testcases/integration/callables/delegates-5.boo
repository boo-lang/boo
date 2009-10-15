"""
added
clicked!
removed
added
clicked!
removed
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

class Application:
	
	def Run():		
		c = Clickable()
		
		for i in range(2):			
			c.Click += clicked
			print("added")
		
			c.RaiseClick()
		
			c.Click -= clicked
			print("removed")
			
			c.RaiseClick()
		
	def clicked(sender, args as System.EventArgs):
		print("clicked!")
		
Application().Run()
