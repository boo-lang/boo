import System

callable StringFunction(item as string) as string

class Handler:
	[property(Prefix)]
	_prefix
	
	def Handle(value):
		return "${_prefix} - ${value}"
		
def apply(items, function as StringFunction):	
	return join(function(item) for item in items, ", ")

value = apply(["foo", "bar"], Handler(Prefix: "zeng").Handle)
assert "zeng - foo, zeng - bar" == value
