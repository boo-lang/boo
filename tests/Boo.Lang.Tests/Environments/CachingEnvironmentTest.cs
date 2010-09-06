using Boo.Lang.Environments;
using Moq;
using NUnit.Framework;

namespace Boo.Lang.Tests.Environments
{
	[TestFixture]
	public class CachingEnvironmentTest
	{
		[Test]
		public void InstancesAreCached()
		{
			const string instance = "42";

			var mock = new Mock<IEnvironment>();
			mock.Setup(e => e.Provide<string>()).Returns(instance).AtMostOnce();

			var subject = new CachingEnvironment(mock.Object);
			ActiveEnvironment.With(subject, ()=>
			{
				Assert.AreSame(instance, My<string>.Instance);
				Assert.AreSame(instance, My<string>.Instance);
			});

			mock.VerifyAll();
		}

		[Test]
		public void CompatibleInstancesAreReturned()
		{
			const string instance = "42";

			var mock = new Mock<IEnvironment>();
			mock.Setup(e => e.Provide<string>()).Returns(instance).AtMostOnce();

			var subject = new CachingEnvironment(mock.Object);
			ActiveEnvironment.With(subject, () =>
			{
				Assert.AreSame(instance, My<string>.Instance);
				Assert.AreSame(instance, My<object>.Instance);
			});

			mock.VerifyAll();
		}
	}
}
