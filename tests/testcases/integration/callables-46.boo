"""
started
executing
done
"""
import System
	
def run(message):
	print(message)
	return 42
	
print("started")

# BeginInvoke is overloaded
# in this example we call the version
# that takes only the original method parameters
result = run.BeginInvoke("executing", null, null)
assert 42 == run.EndInvoke(result)

print("done")
