"""
-Testing...
"""
callable OutputHandler(message as string)

class Printer:
	
	_prefix as string
	
	def constructor(prefix):
		self._prefix = prefix
		
	def print(message as string):
		System.Console.WriteLine("${_prefix}${message}")
	
handler as OutputHandler
handler = Printer("-").print
handler("Testing...")
