#ignore Requires better closure signature inferring
"""
0
"""

import System.Threading
import System.Threading.Tasks
import System

struct Test[of U, V, W]:
    //Regular methods
    public def Foo(f as Func[of Task[of U]]):
        return 1
    public def Foo(f as Func[of Task[of V]]):
        return 2
    public def Foo(f as Func[of Task[of W]]):
        return 3

class TestCase:
    //where there is a conversion between types (int->double)
    [async] public def Run():
        var test = Test[of decimal, string, object]()
        var rez = 0
        // Pick double
        Driver.Tests++
        rez = test.Foo(async({ return 1.0 }))
        if rez == 3: Driver.Count++
        //pick int
        Driver.Tests++
        rez = test.Foo(async({ return 1 }))
        if rez == 1: Driver.Count++
        // The best overload is Func[of Task[of object>>
        Driver.Tests++
        rez = test.Foo(async({ return ""; }))
        if rez == 2: Driver.Count++

class Driver:
    public static Count = 0
    public static Tests = 0

static def Main() as int:
    var t = TestCase()
    t.Run()
    var ret = Driver.Tests - Driver.Count
    Console.WriteLine(ret)
    return ret
