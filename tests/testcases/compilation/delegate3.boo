"""
1 - clicked!
2 - clicked!

"""
import BooCompiler.Tests

class Handler:
	
	public State
		
	def clicked(sender, args as System.EventArgs):
		print("${State} - clicked!")

handler = Handler(State: 1)

c = Clickable()
c.Click += handler.clicked
c.RaiseClick()
c.Click -= handler.clicked

handler.State = 2
c.Click += handler.clicked
c.RaiseClick()
