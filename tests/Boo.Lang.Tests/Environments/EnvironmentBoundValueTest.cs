using System;
using Boo.Lang.Environments;
using NUnit.Framework;

namespace Boo.Lang.Tests.Environments
{
	[TestFixture]
	public class EnvironmentBoundValueTest
	{
		[Test]
		public void SelectRunsInsideOriginalEnvironment()
		{
			var environment = new ClosedEnvironment("42");

			var v = default(EnvironmentBoundValue<string>);
			ActiveEnvironment.With(environment, () => v = EnvironmentBoundValue.Capture<string>());
			
			var actual = v.Select(value => Tuple.Create(value, ActiveEnvironment.Instance)).Value;
			Assert.AreEqual("42", actual.Item1);
			Assert.AreSame(environment, actual.Item2);
		}
	}
}