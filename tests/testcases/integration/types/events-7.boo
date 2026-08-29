"""
Idle called
"""
import System

class Application:
	
	static event Idle as EventHandler
	
	static def Run(o):
		Idle(null, EventArgs.Empty)
	
class Test:
	
	_idleHandler as EventHandler = Application_Idle
	
	def constructor():
		Application.Idle += _idleHandler
		
	def Application_Idle():
		Application.Idle -= _idleHandler
		print "Idle called"

t = Test()
Application.Run(t)
Application.Run(t)
