"""
Exception with type 'System.DivideByZeroException' handled by System.ArithmeticException handler
"""

def ExceptionCaught():
	ExceptionCaught(null, null)
def ExceptionCaught(type as System.Type):
	ExceptionCaught(null, type)
def ExceptionCaught(ex as object):
	ExceptionCaught(ex, null)
def ExceptionCaught(ex as object, exHandler as System.Type):
	value as string
	value += "Anonymous " if ex is null
	value += "Exception "
	value += "with type '${ex.GetType()}' " if ex is not null
	value += "handled by "
	value += "${exHandler} handler" if exHandler is not null
	value += "default handler" if exHandler is null
	print value

try:
	raise System.DivideByZeroException()
except ex as System.NotFiniteNumberException:
	ExceptionCaught(ex, System.NotFiniteNumberException)
except ex as System.ArithmeticException:
	ExceptionCaught(ex, System.ArithmeticException)
except ex:
	ExceptionCaught(ex)
