"""
starting - byrefdelegate.BeginInvoke
executing
called back
done
2 True
starting - normal run.BeginInvoke
executing
called back
done
2 True
"""

import System
import System.Threading

callable byrefdelegate(ref x as int)

def run(ref x as int):
    print("executing")
    ++x

x = 1
print("starting - byrefdelegate.BeginInvoke")
c = run as byrefdelegate
result = c.BeginInvoke(x, { print("called back") }, null)
Thread.Sleep(50ms)
c.EndInvoke(x, result)
print("done")
print x, x==2

x = 1
print("starting - normal run.BeginInvoke")
result2 = run.BeginInvoke(x, { print("called back") }, null)
Thread.Sleep(50ms)
run.EndInvoke(x, result2)
print("done")
print x, x==2

