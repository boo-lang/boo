"""
0
"""

namespace CompilerCrashRepro2

import System
import System.Threading.Tasks

public class Item[of T]:
    [Property(Value)]
    private _value as T

public static class Crasher:
    public def Build[of T]() as Func[of Task[of Item[of T]]]:
        return async({ Item[of T](Value: await(GetValue[of T]())) })

    public def GetValue[of T]() as Task[of T]:
        return Task.FromResult(Default(T))

public def Main():
    var r = Crasher.Build[of int]()().Result.Value
    System.Console.WriteLine(r)
