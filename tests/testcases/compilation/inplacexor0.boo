"""
0
"""
import System.Reflection

x = 1
x ^= 1
print x

f = BindingFlags.Static | BindingFlags.Public
f ^= BindingFlags.Public
assert f == BindingFlags.Static

