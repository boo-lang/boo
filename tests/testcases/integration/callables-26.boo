import System
import NUnit.Framework

callable Function(item) as object

class Handler:
	[property(Prefix)]
	_prefix
	
	def Handle(value):
		return "${_prefix} - ${value}"

fn = cast(Function, Handler(Prefix: "Function").Handle)
Assert.AreEqual("Function - 33", fn(33))