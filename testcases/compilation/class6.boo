"""
clicked from app!

"""
import Boo.Tests.Lang.Compiler from Boo.Tests

class App:

	_clickable as Clickable
	
	def constructor():
		_clickable = Clickable(Click: clicked)
		
	private def clicked(sender, args as System.EventArgs):
		print("clicked from app!")
		
	def Run():
		_clickable.RaiseClick()
		
App().Run()
		
