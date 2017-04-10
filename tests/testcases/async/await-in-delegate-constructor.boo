"""
0
"""

import System
import System.Collections.Generic
import System.Text
import System.Threading
import System.Threading.Tasks

class TestCase:
    static test = 0
    static count = 0

    [async] public static def Run() as Task:
        try:
            test++
            checked:
                var f = Func[of int, object](await(Bar()))
            var x = f(1)
            if (x cast string) != "1":
                count--
        ensure:
            Driver.Result = test - count
            Driver.CompleteSignal.Set()

    [async] static def Bar() as Task[of Converter[of int, string]]:
        count++
        await Task.Delay(1)
        return {p1 as int | return p1.ToString()};

class Driver:
    static public CompleteSignal = AutoResetEvent(false)
    public static Result as int = -1

    public static def Main():
        TestCase.Run()
        CompleteSignal.WaitOne()
        Console.Write(Result)

Driver.Main()