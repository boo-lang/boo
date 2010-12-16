using System;
using Boo.Lang.Environments;
using NUnit.Framework;

namespace Boo.Lang.Tests.Environments
{
	[TestFixture]
	public class DeferredEnvironmentTest
	{
		class Foo {}

		class ImprovedFoo : Foo {}

		class Bar {}

		[Test]
		public void ExactTypeRequestIsFullfilled()
		{
			var environment = new DeferredEnvironment { { typeof(Foo), () => new Foo() } };
			ActiveEnvironment.With(environment, () => Assert.IsNotNull(My<Foo>.Instance));
		}

		[Test]
		public void CompatibleTypeRequestIsFullfilled()
		{
			var environment = new DeferredEnvironment { { typeof(ImprovedFoo), () => new ImprovedFoo() } };
			ActiveEnvironment.With(environment, () => Assert.IsNotNull(My<Foo>.Instance));
		}

		[Test]
		public void IncompatibleTypeRequestIsNotFullfilled()
		{
			var environment = new DeferredEnvironment { { typeof(Foo), () => new Foo() } };
			ActiveEnvironment.With(environment, () =>
			{
				try
				{
					My<Bar>.Instance.ToString();
				}
				catch (InvalidOperationException)
				{
					return;
				}
				Assert.Fail("InvalidOperationException expected");
			});
		}
	}
}
