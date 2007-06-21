"""
void callable
"""
import System

def CallableAsVoid():
	print "void callable"

def CallableAsTimeSpan():
	return DateTime.Now - DateTime.Today

# This creates compiler generated extensions
# for callable() as TimeSpan, including
# BeginInvoke(c as callable() as TimeSpan)
_callableAsTimeSpan = CallableAsTimeSpan

# This creates (and uses) the extension
# BeginInvoke(c as callable() as void)
# which conflicts with the previous
# BeginInvoke(c as callable() as TimeSpan)
handle = CallableAsVoid.BeginInvoke()
CallableAsVoid.EndInvoke(handle)
