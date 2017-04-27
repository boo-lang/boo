"""
"""

namespace A

import System.Threading.Tasks
import System

public struct TestClass:
    [async] public def IntRet(IntI as int) as System.Threading.Tasks.Task[of int]:
        return await(async({ await(Task.Yield()); return IntI })());

static public class B:
    [async] public def MainMethod() as System.Threading.Tasks.Task[of int]:
        var MyRet = 0
        var TC = TestClass()
        if (await((async({ await(Task.Yield()); return (await(TestClass().IntRet(await(async({ await(Task.Yield()); return 3 })()) ))) }) )())  ) !=  await(async({ await(Task.Yield()); return 3 } )()):
            MyRet = 1
        return await(async({await(Task.Yield()); return MyRet})())

def Main():
    B.MainMethod()
    return
