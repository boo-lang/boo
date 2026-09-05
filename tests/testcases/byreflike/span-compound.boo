"""
15
20
"""
import System

buf = (10, 20)
span = buf.AsSpan()
span[0] += 5
for x in buf:
	Console.WriteLine(x)
