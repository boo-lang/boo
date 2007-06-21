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

result = run.BeginInvoke("executing", null, null)
assert 42 == run.EndInvoke(result)

print("done")
