"""
0
"""

import System
import System.Threading
import System.Threading.Tasks

class TestCase:
    [async] public def Run():
        int test = 0
        int result = 0
        try:
            test++
            __switch__({await (async ({ await(Task.Delay(1)); return 5 }))}(), d, d, d, d, d, r)
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
    CompleteSignal.WaitOne()
    Console.WriteLine(Driver.Result)
