"""
Click!
"""
class Clickable:

	event Click as ClickHandler
	
	callable ClickHandler(sender as object, args)
	
c = Clickable()
c.Click += <print("Click!")>
c.Click(null, null)
