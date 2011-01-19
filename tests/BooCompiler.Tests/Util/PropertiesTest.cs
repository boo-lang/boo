using System;
using Boo.Lang.Compiler.Util;
using NUnit.Framework;

namespace BooCompiler.Tests.Util
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
