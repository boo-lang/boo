using Boo.Lang.Environments;
using Moq;
using NUnit.Framework;

namespace Boo.Lang.Tests.Environments
{
	[TestFixture]
	public class EnvironmentProvisionTest
	{
		class Foo {}

		[Test]
		public void ProvisioningHappensOnDemandAndOnlyOnce()
		{
			var mock = new Mock<IEnvironment>();
			mock.Setup(e => e.Provide<Foo>()).Returns(new Foo()).AtMostOnce();

			var foo = new EnvironmentProvision<Foo>();
			ActiveEnvironment.With(mock.Object, () =>
			{
				var first = foo.Instance;
				var second = foo.Instance;
				Assert.IsNotNull(first);
				Assert.AreSame(first, second);
			});

			mock.VerifyAll();
		}
	}
}
