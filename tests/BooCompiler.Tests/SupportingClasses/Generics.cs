namespace BooCompiler.Tests.SupportingClasses
{
	public abstract class AbstractFoo
	{
		public abstract T Bar<T>(T x);
	}

	public abstract class GenericArgumentMustInheritSelf<T> where T : GenericArgumentMustInheritSelf<T>
	{
	}

	public abstract class GenericSelf<T> where T : GenericSelf<T>
	{
	}

	public abstract class GenericSelf<T, S> : GenericSelf<T>
		where T : GenericSelf<T>
		where S : struct
	{
	}
}
