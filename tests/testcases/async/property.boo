"""
0
"""

import System.Threading
import System.Threading.Tasks
import System

class Base:
    private _myProp as int

    public virtual MyProp as int:
        get: return _myProp
        private set: _myProp = value

class TestClass(Base):
    [async] def getBaseMyProp() as Task[of int]:
        await Task.Delay(1)
        return super.MyProp

    [async] public def Run():
        Driver.Result = await(getBaseMyProp())
        Driver.CompleteSignal.Set()

class Driver:
    public static CompleteSignal = AutoResetEvent(false)
    public static Result = -1

public static def Main():
    var tc = TestClass()
    tc.Run()
    Driver.CompleteSignal.WaitOne()
    Console.WriteLine(Driver.Result)
