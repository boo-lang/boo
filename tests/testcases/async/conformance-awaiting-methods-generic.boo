"""
0
"""

import System;
import System.Runtime.CompilerServices;
import System.Threading;

//Implementation of your own async pattern

public class MyTask[of T]:

    public def GetAwaiter() as MyTaskAwaiter[of T]:
        return MyTaskAwaiter[of T]()

    [async] public def Run[of U(MyTask[of int], constructor)](u as U) as void:
        try:
            tests as int = 0
            tests++
            var rez = await(u)
            if rez == 0:
                Driver.Count++
            Driver.Result = Driver.Count - tests
        ensure:
            //When test complete, set the flag.
            Driver.CompletedSignal.Set()

public class MyTaskAwaiter[of T](INotifyCompletion):

    public def OnCompleted(continuationAction as Action) as void:
        pass

    public def GetResult() as T:
        return Default(T)

    public IsCompleted as bool:
        get:
            return true

//-------------------------------------
class Driver:
    public static Result as int = -1
    public static Count as int = 0
    public static CompletedSignal = AutoResetEvent(false)

static def Main():
    MyTask[of int]().Run[of MyTask[of int]](MyTask[of int]())
    Driver.CompletedSignal.WaitOne()
    // 0 - success
    // 1 - failed (test completed)
    // -1 - failed (test incomplete - deadlock, etc)
    Console.WriteLine(Driver.Result)
