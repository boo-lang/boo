import System
import System.Threading

def run():
    print("executing")

print("started")
result = run.BeginInvoke({ print("called back") })
Thread.Sleep(50ms)
run.EndInvoke(result)

print("done")
