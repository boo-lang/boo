namespace Statsia.Tests.StasiaC

import NUnit.Framework
import System.Linq.Enumerable

[TestFixture]
class SampleFixture:
    
    [Test]
    def CosTest():
        // Checks basic array broadcasting for Cos, which has no overloads.
        a = (1.0, 2.0)
        b = System.Math.Cos(a)
        c = array(double, 2)
        for i in range(a.Length):
            c[i] = System.Math.Cos(a[i])
        Assert.AreEqual(b, c)
        
        # Wrong initialized size.
        d = (1.0,)
        d = System.Math.Cos(a)
        Assert.AreEqual(d, c)
        
    [Test]
    def LogTest():
        // Log is overloaded math function
        pass

    
/*
import System.Linq.Enumerable
import System.Math

numbers = (1, 2)
result = Cos(numbers)
for r in result:
    print r
*/