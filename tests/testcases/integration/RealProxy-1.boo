"""
Bar
before Bar
Bar
after Bar
"""
import System
import System.Runtime.Remoting
import System.Runtime.Remoting.Proxies
import System.Runtime.Remoting.Messaging

class Foo(MarshalByRefObject):
	def Bar():
		print "Bar"
		
class TraceProxy(RealProxy):
	
	_target
	
	def constructor(target):
		super(target.GetType())
		_target = target
		
	override def Invoke(message as IMessage):
		call as IMethodCallMessage = message
		print "before", call.MethodName
		returnValue = RemotingServices.ExecuteMessage(_target, message)
		print "after", call.MethodName
		return returnValue
		
f = Foo()
f.Bar()

f = TraceProxy(f).GetTransparentProxy()
f.Bar()
