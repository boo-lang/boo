import System
import NUnit.Framework

s1 as short = 1
s2 as short = 2

a = (s1, s2)
Assert.AreEqual(1, a[0])
Assert.AreEqual(2, a[1])
Assert.AreSame(Type.GetType("System.Int16[]"), a.GetType())
