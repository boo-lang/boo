"""
0
"""

import System
import System.Collections.Generic
import System.Threading
import System.Threading.Tasks

struct Test:
    public Foo as Task[of string]:
        get: return Task.Run[of string](async ({ await(Task.Delay(1)); return "abc" }))

class TestCase[of U]:
    [async] public static def GetValue(x as object) as Task[of object]:
        await Task.Delay(1)
        return x

    public static def GetValue1[of T(Task[of U])](t as T) as T:
        return t
    
    [async] public def Run():
        tests as int = 0
        var t = Test()
        tests++
        var x1 = await(TestCase[of string].GetValue(await(t.Foo)))
        if x1 == "abc":
            Driver.Count++
        tests++
        var x2 = await(TestCase[of string].GetValue1(t.Foo))
        if x2 == "abc":
            Driver.Count++
        Driver.Result = Driver.Count - tests
        //When test completes, set the flag.
        Driver.CompletedSignal.Set()

class Driver:
    public static Result = -1
    public static Count = 0
    public static CompletedSignal = AutoResetEvent(false)

static def Main():
    var t = TestCase[of int]()
    t.Run()
    Driver.CompletedSignal.WaitOne()
    // 0 - success
    // 1 - failed (test completed)
    // -1 - failed (test incomplete - deadlock, etc)
    Console.WriteLine(Driver.Result)
