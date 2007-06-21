"""
Click!
"""
class Clickable:

	event Click as ClickHandler
	
	callable ClickHandler(sender as object, args)
	
class Clickable2(Clickable):
	def RaiseClick():
		self.Click(self, System.EventArgs.Empty)
	
c = Clickable2()
c.Click += { print("Click!") }
c.RaiseClick()
