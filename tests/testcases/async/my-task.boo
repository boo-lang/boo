"""
0
"""

import System
import System.Threading
import System.Threading.Tasks

//Implementation of you own async pattern
public class MyTask:
    [async] public def Run():
        tests as int = 0
        try:
            tests++
            var myTask = MyTask()
            var x = await(myTask)
            if x == 123: Driver.Count++
        ensure:
            Driver.Result = Driver.Count - tests
            //When test complete, set the flag.
            Driver.CompletedSignal.Set()

public class MyTaskAwaiter(System.Runtime.CompilerServices.INotifyCompletion):
    public def OnCompleted(continuationAction as Action):
        pass

    public def GetResult() as int:
        return 123

    public IsCompleted as bool:
        get: return true

[Extension]
public static def GetAwaiter(my as MyTask) as MyTaskAwaiter:
        return MyTaskAwaiter()

//-------------------------------------
class Driver:
    public static Result = -1
    public static Count = 0
    public static CompletedSignal = AutoResetEvent(false)

static def Main():
    MyTask().Run()
    Driver.CompletedSignal.WaitOne()
    // 0 - success
    // 1 - failed (test completed)
    // -1 - failed (test incomplete - deadlock, etc)
    Console.WriteLine(Driver.Result)
