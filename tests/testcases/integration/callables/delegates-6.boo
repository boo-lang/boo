"""
before assignment...
before invocation...
foo!
done.
"""
import System.Threading

def foo():
	print("foo!")
	
function as ThreadStart
print("before assignment...")
function = foo
print("before invocation...")
function()
print("done.")
