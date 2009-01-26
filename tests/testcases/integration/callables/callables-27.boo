import System

callable Function(item) as object

class Handler:
	[property(Prefix)]
	_prefix = null
	
	def Handle(value):
		return "${_prefix} - ${value}"
		
	def GetFunction():
		return self.Handle

fn = cast(Function, Handler(Prefix: "Function").GetFunction())
assert "Function - 33" == fn(33)
