"""
O brave new world...
"""

import System
import System.Diagnostics
import System.Threading.Tasks

[async] public static def F() as Task[of string]:
    return await(Task.Factory.StartNew({ return "O brave new world..." }))

public static def Main():
    t as Task[of string] = F()
    t.Wait(1000 * 3)
    Console.WriteLine(t.Result)
