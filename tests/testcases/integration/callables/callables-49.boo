"""
1st
finished
2nd
finished again
3rd
"""
import System
import System.Threading

def foo(msg):
	print msg
	
def wait(result as IAsyncResult):
	result.AsyncWaitHandle.WaitOne()
	
wait foo.BeginInvoke("1st", { result | print result.AsyncState }, "finished")

e = AutoResetEvent(false)
wait foo.BeginInvoke("2nd", { print "finished again"; e.Set() }, null)
e.WaitOne(500ms)

wait foo.BeginInvoke("3rd", null, null)

