"""
before
clicked!
after
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

def click():
	print("clicked!")

c = Clickable(Click: click)
print("before")
c.RaiseClick()
print("after")
