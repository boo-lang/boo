"""
0
"""

import System
import System.Collections.Generic
import System.Threading
import System.Threading.Tasks

class ObjInit:
    public async as int
    public t as Task
    public l as long

class TestCase:

    private def Throw[of T](i as T) as T:
        MethodCount++
        raise OverflowException()

    [async] private def GetVal[of T](x as T) as Task[of T]:
        await Task.Delay(1)
        Throw(x)
        return x

    [property(MyProperty)]
    private _myProperty as Task[of long]

    [async] public def Run():
        var tests = 0
        t as Task[of int] = Task.Run[of int](async({ await(Task.Delay(1)); raise FieldAccessException(); return 1 }))
        //object type init
        tests++
        try:
            MyProperty = Task.Run[of long](async ({ await(Task.Delay(1)); raise DataMisalignedException(); return 1L }))
            var obj = ObjInit(
                async: await(t), 
                t: GetVal(Task.Run(async({ await(Task.Delay(1))}))),
                l: await(MyProperty) )
            await obj.t
        except as FieldAccessException:
            Driver.Count++
        except:
            Driver.Count--
        Driver.Result = Driver.Count - tests
        //When test complete, set the flag.
        Driver.CompletedSignal.Set()

    public MethodCount = 0

class Driver:
    public static Result = -1
    public static Count = 0
    public static CompletedSignal = AutoResetEvent(false)

static def Main():
    var t = TestCase()
    t.Run()
    Driver.CompletedSignal.WaitOne()
    // 0 - success
    // 1 - failed (test completed)
    // -1 - failed (test incomplete - deadlock, etc)
    Console.WriteLine(Driver.Result)
