"""
QuackSet("", ("param1",), "value1")
QuackGet("", ("param2",))
QuackSet("", ("param3", "param4"), "value2")
QuackGet("", ("param5", "param6"))
QuackSet("Item", ("param7",), "value3")
QuackGet("Item", ("param8",))
QuackSet("Item", ("param9", "param10"), "value4")
QuackGet("Item", ("param11", "param12"))
"""
import System

class Expando(IQuackFu):
	
	def QuackSet(name as string, parameters as (object), value):
		traceMethod("QuackSet", name, parameters, value)

	def QuackGet(name as string, parameters as (object)):
		traceMethod("QuackGet", name, parameters)
		
	def QuackInvoke(name as string, args as (object)) as object:
		pass
		
def traceMethod(name as string, *params):
	print "${name}(${csv(params)})"
	
def repr(o) as string:
	if o isa string: return "\"${o}\""
	return arrayRepr(o)
	
def arrayRepr(a as (object)):
	if len(a) == 1: return "(${repr(a[0])},)"
	return "(${csv(a)})"
	
def csv(collection):
	return join(repr(item) for item in collection, ', ')

e as duck = Expando()
e["param1"] = "value1"
value = e["param2"]

e["param3", "param4"] = "value2"
value = e["param5", "param6"]

e.Item["param7"] = "value3"
value = e.Item["param8"]

e.Item["param9", "param10"] = "value4"
value = e.Item["param11", "param12"]
