#ignore This will fail until Run and RunEx are merged back together
"""
0
"""

import System.Threading
import System.Threading.Tasks
import System

public interface IExplicit:
    def Method(x as int) as Task

class C1(IExplicit):
    [async] def IExplicit.Method(x as int) as Task:
        //This will fail until Run and RunEx are merged back together
        return Task.Run() do():
            await Task.Delay(1)
            Driver.Count++

class TestCase:
    [async] public def Run():
        try:
            tests as int = 0
            tests++
            var c = C1()
            var e = c cast IExplicit
            await e.Method(4)
            Driver.Result = Driver.Count - tests
        ensure:
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
    Console.WriteLine(Driver.Result)
