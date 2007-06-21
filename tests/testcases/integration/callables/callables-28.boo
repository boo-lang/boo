import System

callable Function(item) as object

class Handler:
	[property(Prefix)]
	_prefix
	
	def Handle(value):
		return "${_prefix} - ${value}"
		
	def GetFunction() as Function:
		return self.Handle

fn = Handler(Prefix: "Function").GetFunction()
assert "Function - 33" == fn(33)
