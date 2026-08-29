"""
True
False
False
True
"""
import System.Console

o1 = object()
o2 = object()

WriteLine(o1 is o1)
WriteLine(o1 is o2)
WriteLine(o1 is not o1)
WriteLine(o1 is not o2)
