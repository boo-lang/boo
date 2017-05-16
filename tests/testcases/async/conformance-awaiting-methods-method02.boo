"""
0
"""

import System.Threading
import System.Threading.Tasks
import System

class C:
    public Status as int
    public def constructor():
        pass

interface IImplicit:
    def Method[of T(Task[of C])](*d as (decimal)) as T

class Impl(IImplicit):
    public def Method[of T(Task[of C])](*d as (decimal)) as T:
        //this will fail until Run and RunEx<C> are merged
        aTask = async() do():
            await Task.Delay(1)
            Driver.Count++
            return C(Status: 1)
        return Task.Run(aTask) cast T

class TestCase:
    [async] public def Run():
        try:
            var tests = 0
            var i = Impl()
            tests++
            await i.Method[of Task[of C]](3, 4)
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
