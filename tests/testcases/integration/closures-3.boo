"""
before
clicked!!!
after
"""
import BooCompiler.Tests

c = Clickable()
c.Click += do:
	print("clicked!!!")

print("before")
c.RaiseClick()
print("after")

