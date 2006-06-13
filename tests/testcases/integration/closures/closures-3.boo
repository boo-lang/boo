"""
before
clicked!!!
after
"""
import BooCompiler.Tests

c = Clickable()
c.Click += def:
	print("clicked!!!")

print("before")
c.RaiseClick()
print("after")

