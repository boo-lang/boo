"""
2
1
"""
import System.Console

def last(condition as bool):
	return -1 if condition
	return 0

a = (1, 2)

WriteLine(a[last(true)])
WriteLine(a[last(false)])
