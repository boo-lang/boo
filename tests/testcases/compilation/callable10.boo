"""
started
executing
done
"""
import System
	
def run():
	print("executing")
	
print("started")

result = run.BeginInvoke(null, null)
result.AsyncWaitHandle.WaitOne()
run.EndInvoke(result)

print("done")
