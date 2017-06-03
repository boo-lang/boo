#ignore Requires better closure signature inferring
"""
12
"""

import System.Threading
import System.Threading.Tasks
import System

static class TestCase:
    public def Foo(f as Func[of Task[of double]]):
        return 12
    public def Foo(f as Func[of Task[of object]]):
        return 13

public def Main():
    Console.WriteLine(TestCase.Foo(async({ return 14 })))
