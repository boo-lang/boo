"""
True
1
"""

import System
import System.Collections.Generic
import System.Threading
import System.Threading.Tasks

class Program:
    [async] public def Test() as Task:
        var list = List[of int]() {1, 2, 3}
        using enumerator = list.GetEnumerator():
            Console.WriteLine(enumerator.MoveNext());
            Console.WriteLine(enumerator.Current);
            await Task.Delay(1);

public static def Main():
    Program().Test().Wait()
