"""
"""

import System
import System.Diagnostics
import System.Threading
import System.Threading.Tasks

struct S:
    public A as int

    public def Mutate(b as int) as int:
        A += b
        return 1

static class Test:
    i = 0

    public def G() as Task[of int]:
        return null

    [async] public def F() as Task[of int]:
        var arr = array(S, 10)
        
        return arr[1].Mutate(await(G()))
