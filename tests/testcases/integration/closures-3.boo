"""
before
clicked!!!
after
"""
import BooCompiler.Tests

c = Clickable()
c.Click += callable:
	print("clicked!!!")

print("before")
c.RaiseClick()
print("after")

