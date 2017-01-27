"""
0
"""

import System
import System.Threading.Tasks
import System.Collections.Generic
import System.Threading

class TestCase:
    public static Count = 0

    public static def Foo[of T](t as T) as T:
        return t

    [async] public static def Bar[of T](t as T) as Task[of T]:
        await Task.Delay(1)
        return t

    [async] public static def Run():
        try:
            var x1 = Foo(await(Bar(4)))
            t as Task[of int] = Bar(5)
            x2 as int = Foo(await(t))
            if x1 != 4:
                Count++
            if x2 != 5:
                Count++
        ensure:
            Driver.CompletedSignal.Set()

class Driver:
    public static CompletedSignal = AutoResetEvent(false)

static def Main():
    TestCase.Run()
    Driver.CompletedSignal.WaitOne()
    // 0 - success
    Console.WriteLine(TestCase.Count)
