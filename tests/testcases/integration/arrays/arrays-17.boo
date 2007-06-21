"""
s
p
m
"""
import System
import NUnit.Framework

a1 = "spam".ToCharArray()
print(a1[0])
print(a1[1])
print(a1[-1])

a2 = (a1[0], a1[1])
Assert.AreSame(Type.GetType("System.Char[]"), a2.GetType())
Assert.AreEqual(a1[0], a2[0])
Assert.AreEqual(a1[1], a2[1])
