import System
import NUnit.Framework

a = (cast(byte, 0), cast(byte, 1))

Assert.AreSame(Type.GetType("System.Byte[]"), a.GetType())
Assert.AreEqual(0, a[0])
Assert.AreEqual(1, a[1])
