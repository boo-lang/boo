"""
42
8
"""
import System

buf = (7, 8)
span = buf.AsSpan()
e = span.GetEnumerator()
e.MoveNext()
e.Current = 42
for x in buf:
	Console.WriteLine(x)
