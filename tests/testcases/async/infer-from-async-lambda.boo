"""
System.Threading.Tasks.Task
"""

import System
import System.Threading.Tasks

static class Program:
    public def CallWithCatch[of T](func as Func of T) as T:
        Console.WriteLine(typeof(T).ToString())
        return func()

    [async] public def LoadTestDataAsync() as Task:
        await CallWithCatch(async({await(LoadTestData())}))

    [async] private def LoadTestData() as Task:
        await Task.Run({ })

public def Main(args as (string)):
    var t = Program.LoadTestDataAsync()
    t.Wait(1000)
