"""
123
abc
"""
import System.Console

items = [(1, 0), (3, 0), (2, 0)]
items.Sort()

for a, b in items:
	Write(a)
WriteLine()

items = [(1, "c"), (1, "b"), (1, "a")]
items.Sort()

for a, b in items:
	Write(b)


