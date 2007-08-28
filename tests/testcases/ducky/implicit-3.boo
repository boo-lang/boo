"""
gate is open
gate is closed
"""
namespace Foo

interface Gate:
	Open as bool:
		get
		
class GateAPI:
	static def op_Implicit(g as Gate) as bool:
		return g.Open
		
class OpenGate(GateAPI, Gate):
	Open:
		get: return true
		
class ClosedGate(GateAPI, Gate):
	Open:
		get: return false
	
class Castle:
	
	_gate as Gate
	
	def constructor(gate as Gate):
		_gate = gate
	
	[DuckTyped]
	Gate:
		get: return _gate
		
	def Test():
		if Gate:
			print "gate is open"
		else:
			print "gate is closed"
			
Castle(OpenGate()).Test()
Castle(ClosedGate()).Test()
