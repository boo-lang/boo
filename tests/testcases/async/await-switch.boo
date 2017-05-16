"""
0
"""

import System
import System.Threading
import System.Threading.Tasks

class TestCase:
    [async] public def Run() as void:
        test as int = 0
        result as int = 0
        try:
            test++
            __switch__(await (async ({ await(Task.Delay(1)); return 5 })()), d, d, d, d, d, r)
            goto d
            :r
            result++
            :d
        ensure:
            Driver.Result = test - result
            Driver.CompleteSignal.Set()

class Driver:
    static public CompleteSignal = AutoResetEvent(false)
    public static Result = -1

public static def Main():
    var tc = TestCase()
    tc.Run()
    Driver.CompleteSignal.WaitOne()
    Console.WriteLine(Driver.Result)
