"""
1st
finished
2nd
finished again
3rd
"""
import System

def foo(msg):
	print msg
	
def wait(result as IAsyncResult):
	result.AsyncWaitHandle.WaitOne()
	
wait(foo.BeginInvoke("1st", { result as IAsyncResult | print result.AsyncState }, "finished"))

wait(foo.BeginInvoke("2nd", { print "finished again" }))

wait(foo.BeginInvoke("3rd"))

