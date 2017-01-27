"""
1
"""

public struct GenC[of T(struct)]:
    public valueN as T?
    [async] public def Test(t as T):
        valueN = t;

public static def Main():
    var test = 12
    var _int = GenC[of int]()
    _int.Test(test)
    System.Console.WriteLine((_int.valueN if _int.valueN.HasValue else 1))
