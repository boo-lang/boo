"""
Bar
before Bar
Bar
after Bar
"""
import System
import System.Reflection

interface IFoo:
	def Bar()

class Foo(IFoo):
	def Bar():
		print "Bar"

class TraceProxy(DispatchProxy):

	_target as IFoo

	static def Wrap(target as IFoo) as IFoo:
		proxy = DispatchProxy.Create[of IFoo, TraceProxy]()
		cast(TraceProxy, proxy)._target = target
		return proxy

	protected override def Invoke(method as MethodInfo, args as (object)) as object:
		print "before", method.Name
		returnValue = method.Invoke(_target, args)
		print "after", method.Name
		return returnValue

f as IFoo = Foo()
f.Bar()

f = TraceProxy.Wrap(f)
f.Bar()
