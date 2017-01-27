"""
0
"""

using System
using System.Collections.Generic
using System.Text
using System.Threading
using System.Threading.Tasks

class TestCase:
    static test = 0
    static count = 0

    [async] public static def Run() as Task:
        try:
            test++
            f as Func[of int, object] = {checked: await(Bar())}
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
    public static int Result = -1

    public static def Main():
        TestCase.Run()
        CompleteSignal.WaitOne()
        Console.Write(Result)
