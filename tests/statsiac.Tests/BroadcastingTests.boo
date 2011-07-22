namespace Statsia.Tests.StasiaC

import NUnit.Framework
import System.Linq.Enumerable
import Statsia.Math

[TestFixture]
class SampleFixture:
    
    [Test]
    def CosTest():
        // Checks basic array broadcasting for Cos, which has no overloads.
        a = (1.0, 2.0)
        b = cos(a)
        c = array(double, 2)
        for i in range(a.Length):
            c[i] = cos(a[i])
        Assert.AreEqual(b, c)
                
        # Wrong initialized size.
        d = (1.0,)
        d = cos(a)
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