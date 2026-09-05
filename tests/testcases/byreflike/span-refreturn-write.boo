"""
99
8
"""
import System

buf = (7, 8)
span = buf.AsSpan()
span.GetPinnableReference() = 99
for x in buf:
	Console.WriteLine(x)
