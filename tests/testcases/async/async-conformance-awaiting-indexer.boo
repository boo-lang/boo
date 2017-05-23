#category FailsOnMono4
"""
0
"""

import System.Threading
import System.Threading.Tasks
import System

struct MyStruct[of T(Task[of Func[of int]])]:
    property t as T

    public self[index as T] as T:
        get:
            return t
        set:
            t = value

struct TestCase:
    public static Count = 0
    private tests as int

    [async] public def Run():
        self.tests = 0
        var ms = MyStruct[of Task[of Func[of int]]]()
        try:
            ms[null] = Task.Run[of Func[of int]](async({ await(Task.Delay(1)); Interlocked.Increment(TestCase.Count); return {123} }))
            self.tests++
            var x = await(ms[await(Foo(null))])
            if x() == 123:
                self.tests++
        ensure:
            Driver.Result = TestCase.Count - self.tests
            //When test complete, set the flag.
            Driver.CompletedSignal.Set()

    [async] public def Foo(d as Task[of Func[of int]]) as Task[of Task[of Func[of int]]]:
        await Task.Delay(1)
        Interlocked.Increment(TestCase.Count)
        return d

class Driver:
    public static Result = -1
    public static CompletedSignal = AutoResetEvent(false)

def Main():
    var t = TestCase()
    t.Run()
    Driver.CompletedSignal.WaitOne()
    // 0 - success
    // 1 - failed (test completed)
    // -1 - failed (test incomplete - deadlock, etc)
    Console.WriteLine(Driver.Result)
