namespace BooCompiler.Tests.SupportingClasses
{
	public interface BaseInterface
	{
		void Add(string s);
	}
	
	public abstract class BaseAbstractClassWithoutImplementation
	{
		public void Add(int i)
		{
		}
	}
	
	public abstract class BaseAbstractClassWithImplementation
	{
		public void Add(string s)
		{
		}
	}
}
