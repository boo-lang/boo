"""
before
clicked!!!
after
"""
import BooCompiler.Tests

click = def:
	print("clicked!!!")

c = Clickable(Click: click)
print("before")
c.RaiseClick()
print("after")

