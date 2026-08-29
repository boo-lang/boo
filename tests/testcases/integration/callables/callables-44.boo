"""
started
executing
called back
done
"""
import System
	
def run(message):
	print(message)
	return 42
	
print("started")

# BeginInvoke is overloaded
# in this example we call the version
# that takes only the callback after the method's parameters
result = run.BeginInvoke("executing", { print("called back") }, null)
System.Threading.Thread.Sleep(50ms)
assert 42 == run.EndInvoke(result)

print("done")
