namespace BooExplorer.Common

import System
import System.IO

class ConsoleCapture(IDisposable):	
	_console = StringWriter()
	_old
	
	def constructor():
		_old = Console.Out
		Console.SetOut(_console)
		
	override def ToString():
		return _console.ToString()
	
	def Dispose():
		Console.SetOut(_old)
