"""
before
clicked!!!
after
"""
import BooCompiler.Tests

click = callable:
	print("clicked!!!")

c = Clickable(Click: click)
print("before")
c.RaiseClick()
print("after")

