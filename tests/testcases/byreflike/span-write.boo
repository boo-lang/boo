"""
1
2
3
"""
import System

buf = array(int, 3)
span = buf.AsSpan()
for i in range(span.Length):
	span[i] = i + 1
for x in buf:
	Console.WriteLine(x)
