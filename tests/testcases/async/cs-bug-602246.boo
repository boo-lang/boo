"""
12
"""

import System
import System.Threading.Tasks

public static class TestCase:
    [async] public def Run[of T](t as T) as Task[of T]:
        await Task.Delay(1)
        f as Func[of Func[of Task[of T]], Task[of T]] = async({x | return await(x()) })
        var rez = await(f(async({ await(Task.Delay(1)); return t })))
        return rez;

public def Main() as void:
    var t = TestCase.Run[of int](12)
    if not t.Wait(1000): raise Exception()
    Console.Write(t.Result)
