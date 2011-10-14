"""
s
p
m
"""
import System

a1 = "spam".ToCharArray()
print(a1[0])
print(a1[1])
print(a1[-1])

a2 = (a1[0], a1[1])
assert Type.GetType("System.Char[]") is a2.GetType()
assert a1[0] == a2[0]
assert a1[1] == a2[1]
