"""
7
7
7
"""
import System

def fill(s as Span[of int], v as int):
	for i in range(s.Length):
		s[i] = v

buf = array(int, 3)
fill(buf.AsSpan(), 7)
for x in buf:
	Console.WriteLine(x)
