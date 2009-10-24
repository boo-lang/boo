namespace BooCompiler.Tests.SupportingClasses
{
	public interface BaseInterface
	{
		void Add(string s);
	}

	public abstract class AbstractClassWithExplicitInterfaceImpl : BaseInterface
	{
		protected abstract void Add(string s);

		void BaseInterface.Add(string s)
		{
			Add(s);
		}
	}
	
	public abstract class BaseAbstractClassWithoutImplementation
	{
		public void Add(int i)
		{
		}
	}
	
	public abstract class BaseAbstractClassWithImplementation
	{
		public void Add(object o)
		{
		}

		public void Add(string s)
		{
		}
	}
}
