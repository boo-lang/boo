"""
2
8
"""
import System

def whole(a as (int)) as Span[of int]:
	return a.AsSpan()

def bump(ref s as Span[of int]):
	s[0] = s[0] + 1

buf = (1, 8)
s = whole(buf)
bump(s)
for x in buf:
	Console.WriteLine(x)
