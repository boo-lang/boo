"""
0
"""

import System
import System.Threading
import System.Threading.Tasks

struct TestCase:
    private t as Task[of int]

    [async] public def Run():
        tests as int = 0
        try:
            tests++
            t = Task.Run(async({ await(Task.Delay(1)); return 1 }))
            var x = await(t)
            if x == 1: Driver.Count++;
            tests++
            t = Task.Run(async({ await(Task.Delay(1)); return 1 }))
            var x2 = await(self.t)
            if x2 == 1: Driver.Count++
        ensure:
            Driver.Result = Driver.Count - tests
            //When test complete, set the flag.
            Driver.CompletedSignal.Set()

class Driver:
    public static Result = -1
    public static Count = 0
    public static CompletedSignal = AutoResetEvent(false)

static def Main():
    var t = TestCase()
    t.Run()
    Driver.CompletedSignal.WaitOne()
    // 0 - success
    // 1 - failed (test completed)
    // -1 - failed (test incomplete - deadlock, etc)
    Console.Write(Driver.Result)
