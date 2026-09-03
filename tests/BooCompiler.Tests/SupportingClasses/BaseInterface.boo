namespace BooCompiler.Tests.SupportingClasses

interface BaseInterface:
	def Add(s as string)

abstract class AbstractClassWithExplicitInterfaceImpl(BaseInterface):
	protected abstract def Add(s as string):
		pass

	def BaseInterface.Add(s as string):
		Add(s)

abstract class BaseAbstractClassWithoutImplementation:
	public def Add(i as int):
		pass

abstract class BaseAbstractClassWithImplementation:
	public def Add(o as object):
		pass

	public def Add(s as string):
		pass
