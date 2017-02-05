"""
42
"""

import System
import System.Threading
import System.Threading.Tasks

class Test:
    internal static i = 0

    [async] public static def F(handle as AutoResetEvent):
        await Task.Factory.StartNew({ Test.i = 42 })
        handle.Set()

public static def Main():
    var handle = AutoResetEvent(false)
    Test.F(handle)
    handle.WaitOne(1000)
    Console.WriteLine(Test.i)
