"""
before
clicked!!!
after
"""
import BooCompiler.Tests

click = do:
	print("clicked!!!")

c = Clickable(Click: click)
print("before")
c.RaiseClick()
print("after")

