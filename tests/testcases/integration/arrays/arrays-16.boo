import System

s1 as short = 1
s2 as short = 2

a = (s1, s2)
assert 1 ==  a[0]
assert 2 ==  a[1]
assert Type.GetType("System.Int16[]") is a.GetType()
