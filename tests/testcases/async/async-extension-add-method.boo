"""
GetVal 1
Add 1
Add 2
Add 3
"""

import System
import System.Collections.Generic
import System.Threading
import System.Threading.Tasks

[Extension]
public def Add[of T](stack as Stack[of T], item as T):
    Console.WriteLine("Add $item")
    stack.Push(item)

class TestCase:
    public handle = AutoResetEvent(false)

    [async] private def GetVal[of T](x as T) as Task[of T]:
        await Task.Delay(1)
        Console.WriteLine("GetVal $x")
        return x

    [async] public def Run():
        try:
            var stack = Stack[of int]() {await(GetVal(1)), 2, 3 }
        ensure:
            handle.Set()

public def Main(args as (string)):
    var tc = TestCase()
    tc.Run()
    tc.handle.WaitOne(1000)