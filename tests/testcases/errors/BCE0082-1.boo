"""
BCE0082-1.boo(10,8): BCE0082: Event 'BooCompiler.Tests.Clickable.Click' can not be externally raised.
"""
import System

class Button:
	event Click as EventHandler
	
c = Button()
c.Click(c, EventArgs.Empty)
