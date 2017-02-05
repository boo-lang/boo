"""
0
"""

import System
import System.Collections.Generic
import System.Threading.Tasks
import System.Threading

class TestCase(Test):
    public static Count = 0

    [async] public static def Run():
        try:
            x as int = await(Test.GetValue[of int](1))
            if x != 1:
                Count++
        ensure:
            Driver.CompletedSignal.Set()

class Test:
    [async] protected static def GetValue[of T](t as T) as Task[of T]:
        await Task.Delay(1)
        return t

class Driver:
    public static CompletedSignal = AutoResetEvent(false)

static def Main():
    TestCase.Run()
    Driver.CompletedSignal.WaitOne()
    // 0 - success
    Console.WriteLine(TestCase.Count)
