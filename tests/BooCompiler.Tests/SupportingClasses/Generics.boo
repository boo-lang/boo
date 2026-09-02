namespace BooCompiler.Tests.SupportingClasses

abstract class AbstractFoo:
	public abstract def Bar[of T](x as T) as T:
		pass

abstract class GenericArgumentMustInheritSelf[of T(GenericArgumentMustInheritSelf[of T])]:
	pass

abstract class GenericSelf[of T(GenericSelf[of T])]:
	pass

abstract class GenericSelf[of T(GenericSelf[of T]), S(struct)](GenericSelf[of T]):
	pass
