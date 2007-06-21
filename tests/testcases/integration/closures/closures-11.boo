"""
before
clicked!!!
after
"""
import BooCompiler.Tests

c = Clickable()
c.Click += { print("clicked!!!") }

print("before")
c.RaiseClick()
print("after")

