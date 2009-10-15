"""
before
clicked!!!
after
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

click = def:
	print("clicked!!!")

c = Clickable(Click: click)
print("before")
c.RaiseClick()
print("after")

