"""
Pre
show1
show2
disposed2
disposed1
Post
result
"""

import System.Threading.Tasks
import System

class Program:
    class D(IDisposable):
        public def Dispose():
            print "disposed1"
    class D2(IDisposable):
        public def Dispose():
            print "disposed2"

    [async] static def M(input as int) as Task[of string]:
        print "Pre"
        using window1 = D():
            print "show1"
            using window = D2():
                print "show2"
                for i in range(2):
                    await Task.Delay(100)
        print "Post"
        return "result"
    
static def Main():
    System.Console.WriteLine(Program.M(0).Result)
