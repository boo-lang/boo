"""
foo
DEADLOCK DETECTED
"""

import System
import Boo.Lang.Compiler

class Deadlocker:

	[getter(DaLock)]
	_daLock = object()

	def Foo():
		lock _daLock:
			print "foo"

macro enableDeadLockDetectorToKickInAt200ms:
	Context.Parameters.Defines.Add("LOCK_TIMEOUT", "200")

enableDeadLockDetectorToKickInAt200ms
try:
	d = Deadlocker()
	d.Foo()
	lock d.DaLock:
		res = d.Foo.BeginInvoke()
		d.Foo.EndInvoke(res)
except as System.TimeoutException:
	print "DEADLOCK DETECTED"

