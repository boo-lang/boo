"""
started
executing
callback
done
"""
import System

def callback(result as IAsyncResult):
	print("callback")
	
def run():
	print("executing")
	
print("started")

result = run.BeginInvoke(callback, null)
System.Threading.Thread.Sleep(50ms)
run.EndInvoke(result)

print("done")
