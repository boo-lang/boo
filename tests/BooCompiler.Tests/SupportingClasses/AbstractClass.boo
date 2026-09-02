namespace BooCompiler.Tests.SupportingClasses

abstract class AbstractClass:
	pass

abstract class AnotherAbstractClass:
	protected abstract def Foo() as string:
		pass

	public virtual def Bar() as string:
		return "Bar"

abstract class A1:
	public abstract P1 as int:
		get

abstract class A2(A1):
	pass
