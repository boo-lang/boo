"""
Click!
"""
class Clickable:

	event Click as callable(object, object)
	
	def RaiseClick():
		Click(self, null)
	
c = Clickable()
c.Click += { print("Click!") }
c.RaiseClick()
