"""
H
i
c=H
c=i
-1
-2
"""
import System

for c in "Hi".AsSpan():
	Console.WriteLine(c.ToString())

for c in "Hi".AsSpan():
	Console.WriteLine("c=${c}")

buf = (1, 2)
for v in buf.AsSpan():
	Console.WriteLine(-v)
