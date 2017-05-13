"""
0
"""

import System.Threading
import System.Threading.Tasks
import System

class TestCase:
    public static Count = 0

    [async] public def Run():
        var tests = 0
        var x1 = await(Foo1()) isa object
        var x2 = await(Foo2()) as string
        if x1 == true:
            tests++
        if x2 == "string":
            tests++
        Driver.Result = TestCase.Count - tests
        //When test complete, set the flag.
        Driver.CompletedSignal.Set()

    [async] public def Foo1() as Task[of int]:
        await Task.Delay(1)
        TestCase.Count++
        var i = 0
        return i

    [async] public def Foo2() as Task[of object]:
        await Task.Delay(1)
        TestCase.Count++
        return "string"

class Driver:
    public static Result = -1
    public static CompletedSignal = AutoResetEvent(false)

def Main():
    var t = TestCase()
    t.Run()
    Driver.CompletedSignal.WaitOne()
    // 0 - success
    // 1 - failed (test completed)
    // -1 - failed (test incomplete - deadlock, etc)
    Console.Write(Driver.Result)
