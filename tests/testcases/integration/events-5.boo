"""
Click!
"""
class Clickable:

	event Click as callable(object, object)
	
c = Clickable()
c.Click += { print("Click!") }
c.Click(null, null)
