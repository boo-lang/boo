"""
before
clicked!
after
"""
import BooCompiler.Tests

def click(sender, args as System.EventArgs):
	print("clicked!")

c = Clickable(Click: System.EventHandler(null, __addressof__(click)))

print("before")
c.RaiseClick()
print("after")
