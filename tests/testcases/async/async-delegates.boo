"""
0
2
"""

import System
import System.Threading.Tasks

static class Program:
    def test1():
        value = async() do():
            if 0.ToString().Length == 0:
                await Task.Yield()
            else:
                System.Console.WriteLine(0.ToString())
        Invoke(value)

    def test2() as string:
        value = async() do():
            if 0.ToString().Length == 0:
                await Task.Yield()
                return 1.ToString()
            else:
                System.Console.WriteLine(2.ToString())
                return null
        return Invoke(value);

    def Invoke(method as Action):
        method()

    def Invoke(method as Func[of Task]):
        method().Wait();

    def Invoke[of TResult](method as Func[of TResult]) as TResult:
        return method()

    internal def Invoke[of TResult](method as Func[of Task[of TResult]]) as TResult:
        if method != null:
            return Invoke1(async({ await(Task.Yield()); return await(method()) }))
        return Default(TResult)

    internal static def Invoke1[of TResult](method as Func[of Task[of TResult]]) as TResult:
        return method().Result

def Main(args as (string)):
    Program.test1()
    Program.test2()
