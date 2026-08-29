"""
Click!
"""
class Clickable:

	event Click as ClickHandler

	callable ClickHandler(sender as object, args)

	protected def RaiseClick():
		self.Click(self, System.EventArgs.Empty)

class Clickable2(Clickable):
	def DoRaiseClick():
		RaiseClick()

c = Clickable2()
c.Click += { print("Click!") }
c.DoRaiseClick()
