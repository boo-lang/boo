import System

def run():
    print("executing")

print("started")
result = run.BeginInvoke({ print("called back") })
System.Threading.Thread.Sleep(50ms)
run.EndInvoke(result)

print("done")
