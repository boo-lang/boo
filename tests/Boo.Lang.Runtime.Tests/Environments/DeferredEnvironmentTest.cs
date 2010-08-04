using System;
using Boo.Lang.Environments;
using NUnit.Framework;
using Environment = Boo.Lang.Environments.Environment;

namespace Boo.Lang.Runtime.Tests.Environments
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
			Environment.With(environment, () => Assert.IsNotNull(My<Foo>.Instance));
		}

		[Test]
		public void CompatibleTypeRequestIsFullfilled()
		{
			var environment = new DeferredEnvironment { { typeof(ImprovedFoo), () => new ImprovedFoo() } };
			Environment.With(environment, () => Assert.IsNotNull(My<Foo>.Instance));
		}

		[Test]
		public void IncompatibleTypeRequestIsNotFullfilled()
		{
			var environment = new DeferredEnvironment { { typeof(Foo), () => new Foo() } };
			Environment.With(environment, () =>
			{
				try
				{
					var bar = My<Bar>.Instance;
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
