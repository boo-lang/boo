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
        nullLambda = do():
            pass
        await Task.Run(nullLambda)

public def Main(args as (string)):
    var t = Program.LoadTestDataAsync()
    t.Wait(1000)
