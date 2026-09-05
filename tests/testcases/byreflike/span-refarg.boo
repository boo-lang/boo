"""
2
8
"""
import System

def bump(ref x as int):
	x += 1

buf = (1, 8)
span = buf.AsSpan()
bump(span[0])
for x in buf:
	Console.WriteLine(x)
