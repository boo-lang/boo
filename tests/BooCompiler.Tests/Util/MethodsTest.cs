using System;
using System.Collections;
using Boo.Lang.Compiler.Util;
using NUnit.Framework;

namespace BooCompiler.Tests.Util
{
	[TestFixture]
	public class MethodsTest
	{
		[Test]
		public void InstanceActionOfNoArgs()
		{
			Assert.AreSame(typeof(IDisposable).GetMethod("Dispose"), Methods.InstanceActionOf<IDisposable>(d => d.Dispose));
		}

		[Test]
		public void InstanceMethodOfForFunction()
		{
			Assert.AreSame(typeof(IEnumerable).GetMethod("GetEnumerator"), Methods.InstanceFunctionOf<IEnumerable, IEnumerator>(e => e.GetEnumerator));
		}

		[Test]
		public void GetterOf()
		{
			Assert.AreSame(typeof(string).GetProperty("Length").GetGetMethod(), Methods.GetterOf<string, int>(s => s.Length));
		}

		[Test]
		public void ConstructorOf()
		{
			Assert.AreSame(typeof(ArgumentOutOfRangeException).GetConstructor(new[] { typeof(string) }),
			               Methods.ConstructorOf(() => new ArgumentOutOfRangeException(string.Empty)));
		}
	}
}
