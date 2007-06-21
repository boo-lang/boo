import NUnit.Framework

callable OutputHandler(message as string) as string

class Printer:
	
	_prefix as string
	
	def constructor(prefix):
		self._prefix = prefix
		
	def print(message as string):
		return "${_prefix}${message}"
	
handler as OutputHandler
handler = Printer("-").print
call = handler.BeginInvoke("Testing...", null, null)
call.AsyncWaitHandle.WaitOne()
value = handler.EndInvoke(call)

Assert.AreEqual("-Testing...", value)
