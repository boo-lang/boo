"""
0
"""

import System.Threading
import System.Threading.Tasks
import System

callable MyDel[of U](ref u as U) as Task

class MyClass[of T]:
    public static def Meth(ref t as T) as Task:
        t = Default(T)
        return Task.Run(async ({ await(Task.Delay(1)); TestCase.Count++ }))

    public myDel as MyDel[of T]

    public event myEvent as MyDel[of T]

    [async] public def TriggerEvent(p as T) as Task:
        try:
            await myEvent(p)
        except:
            TestCase.Count += 5

struct TestCase:
    public static Count = 0
    private tests as int

    [async] public def Run():
        tests = 0
        try:
            tests++;
            var ms = MyClass[of string]()
            ms.myDel = MyClass[of string].Meth;
            var str = ""
            await ms.myDel(str)
            tests++
            ms.myEvent += MyClass[of string].Meth
            await ms.TriggerEvent(str)
        ensure:
            Driver.Result = TestCase.Count - self.tests
            //When test complete, set the flag.
            Driver.CompletedSignal.Set()

class Driver:
    public static Result = -1
    public static CompletedSignal = AutoResetEvent(false)

static def Main():
    var t = TestCase()
    t.Run()
    Driver.CompletedSignal.WaitOne()
    // 0 - success
    // 1 - failed (test completed)
    // -1 - failed (test incomplete - deadlock, etc)
    Console.WriteLine(Driver.Result)
