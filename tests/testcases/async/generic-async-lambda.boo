"""
12
"""

import System
import System.Diagnostics
import System.Threading
import System.Threading.Tasks

class G[of T]:
    t as T

    public def constructor(t as T, action as Func[of T, Task[of T]]):
        var tt = action(t)
        var completed = tt.Wait(1000)
        Debug.Assert(completed)
        self.t = tt.Result

    public override def ToString() as string:
        return t.ToString()

static class Test:
    def M[of U](t as U) as G[of U]:
        return G[of U](t, async({x | return await(IdentityAsync(x)) }))

    [async] def IdentityAsync[of V](x as V) as Task[of V]:
        await Task.Delay(1)
        return x

public def Main():
    var g = Test.M(12)
    Console.WriteLine(g)
