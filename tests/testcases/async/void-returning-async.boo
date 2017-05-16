"""
1
"""

import System
import System.Diagnostics
import System.Threading
import System.Threading.Tasks

class Test:

    public static i as int = 0

    [async] public static def F(handle as AutoResetEvent) as void:
        try:
            await Task.Factory.StartNew({ Interlocked.Increment(Test.i) })
        ensure:
            handle.Set()

public static def Main():
    var handle = AutoResetEvent(false)
    Test.F(handle)
    handle.WaitOne(1000 * 3)
    Console.WriteLine(Test.i)
