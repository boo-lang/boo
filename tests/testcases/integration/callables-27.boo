import System
import NUnit.Framework

callable Function(item) as object

class Handler:
	[property(Prefix)]
	_prefix
	
	def Handle(value):
		return "${_prefix} - ${value}"
		
	def GetFunction():
		return self.Handle

fn = cast(Function, Handler(Prefix: "Function").GetFunction())
Assert.AreEqual("Function - 33", fn(33))