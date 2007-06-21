"""
before
clicked!!!
after
"""
import BooCompiler.Tests

c = Clickable(Click: { print("clicked!!!") })
print("before")
c.RaiseClick()
print("after")

