import System
import System.Collections
import NUnit.Framework

generator = i*2 for i in range(1, 10)

e1 = generator.GetEnumerator()

Assert.IsTrue(e1 isa ICloneable, "enumerator must be cloneable!")

Assert.IsTrue(e1.MoveNext())
Assert.AreEqual(2, e1.Current)

e2 as IEnumerator = cast(ICloneable, e1).Clone()
Assert.IsFalse(e1 is e2)
Assert.AreEqual(2, e2.Current)

Assert.IsTrue(e1.MoveNext())
Assert.AreEqual(4, e1.Current)
Assert.AreEqual(2, e2.Current, "Clone must not be affected by original copy!")

