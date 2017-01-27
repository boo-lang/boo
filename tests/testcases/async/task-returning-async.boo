"""
42
"""

import System
import System.Diagnostics
import System.Threading.Tasks

class Test:
    public static i as int = 0
    [async] public static def F() as Task:
        await(Task.Factory.StartNew({ Test.i = 42} ))

public static def Main():
    t as Task = Test.F()
    t.Wait(1000)
    Console.WriteLine(Test.i)
