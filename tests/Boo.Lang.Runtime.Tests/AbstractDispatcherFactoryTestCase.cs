using NUnit.Framework;

namespace Boo.Lang.Runtime.Tests
{
	public abstract class AbstractDispatcherFactoryTestCase
	{
		protected ExtensionRegistry _extensions;

		[SetUp]
		public void SetUp()
		{	
			_extensions = new ExtensionRegistry();
			_extensions.Register(typeof(FooExtensions));
		}
	}
}