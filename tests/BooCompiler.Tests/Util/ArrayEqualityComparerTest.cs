using System.Collections.Generic;
using Boo.Lang.Compiler.Util;
using NUnit.Framework;

namespace BooCompiler.Tests.Util
{
	[TestFixture]
	public class ArrayEqualityComparerTest
	{
		[Test]
		public void StringEquals()
		{
			IEqualityComparer<string[]> comparer = ArrayEqualityComparer<string>.Default;

			string[] a1 = new string[] {"foo", "bar"};
			string[] a2 = new string[] {new string("foo".ToCharArray()), "bar"};
			Assert.IsTrue(comparer.Equals(a1, a2));
		}

		[Test]
		public void ByteEquals()
		{
			IEqualityComparer<byte[]> comparer = ValueTypeArrayEqualityComparer<byte>.Default;

			byte[] a1 = new byte[] {0, 42};
			byte[] a2 = new byte[] {0, 42};
			Assert.IsTrue(comparer.Equals(a1, a2));
		}
	}
}
