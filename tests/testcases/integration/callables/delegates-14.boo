"""
before
clicked!
after
"""
import BooCompiler.Tests

def click():
	print("clicked!")

c = Clickable()
c.Click += click

print("before")
c.RaiseClick()
print("after")
