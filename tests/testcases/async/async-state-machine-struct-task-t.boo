"""
42
"""

import System
import System.Threading.Tasks

class Test:
    [async] public static def F() as Task of int:
        return await(Task.Factory.StartNew({42}))

public def Main():
    var t = Test.F()
    t.Wait()
    Console.WriteLine(t.Result)
