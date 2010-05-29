using System;
using NUnit.Framework;

namespace Boo.Lang.Runtime.Tests
{
	[TestFixture]
	public class PropertiesTest
	{
		[Test]
		public void Of()
		{
			Assert.AreSame(typeof(Array).GetProperty("Length"), Properties.Of<Array, int>(a => a.Length));
		}
	}
}
