import System

callable Function(item) as object

class Handler:
	[property(Prefix)]
	_prefix = null
	
	def Handle(value):
		return "${_prefix} - ${value}"

fn = cast(Function, Handler(Prefix: "Function").Handle)
assert "Function - 33" == fn(33)
