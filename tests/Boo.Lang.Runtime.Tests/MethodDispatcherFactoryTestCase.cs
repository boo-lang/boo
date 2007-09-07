using System;
using NUnit.Framework;

namespace Boo.Lang.Runtime.Tests
{
	class Foo
	{
		public string Echo(string value)
		{
			return "Echo: " + value;
		}

		public static string StaticEcho(string value)
		{
			return "StaticEcho: " + value;
		}
	}

	class FooExtensions
	{
		[Boo.Lang.Extension]
		public static string ExtensionEcho(Foo self, string value)
		{
			return "ExtensionEcho: " + self.Echo(value);
		}

		[Boo.Lang.Extension]
		public static string ExtensionEchoVar(Foo self, int i, params string[] value)
		{
			return "ExtensionEchoVar(" + i + ", " + string.Join(", ", value) + ")";
		}
	}

	[TestFixture]
	public class MethodDispatcherFactoryTestCase : AbstractDispatcherFactoryTestCase
	{
		[Test]
		public void ExtensionMethod()
		{
			Foo o = new Foo();
			object value = Dispatch(o, "ExtensionEcho", "Hello");
			Assert.AreEqual(FooExtensions.ExtensionEcho(o, "Hello"), value);
		}

		[Test]
		public void ExtensionMethodWithVarArgs()
		{
			Foo o = new Foo();
			object value = Dispatch(o, "ExtensionEchoVar", 1, "skip", "Hello");
			Assert.AreEqual(FooExtensions.ExtensionEchoVar(o, 1, "skip", "Hello"), value);
		}

		[Test]
		public void InstanceMethod()
		{
			Foo o = new Foo();
			object value = Dispatch(o, "Echo", "Hello");
			Assert.AreEqual(o.Echo("Hello"), value);
		}

		private object Dispatch(Foo o, string methodName, params object[] args)
		{
			return Dispatch(o, o.GetType(), methodName, args);
		}

		private object Dispatch(Foo o, Type type, string methodName, params object[] args)
		{
			return new MethodDispatcherFactory(_extensions, o, type, methodName, args).Create()(o, args);
		}

		[Test]
		public void StaticMethod()
		{
			AssertStaticMethod(new Foo());
			AssertStaticMethod(null);
			AssertStaticMethod(new Foo());
		}

		private void AssertStaticMethod(Foo target)
		{
			object result = Dispatch(target, typeof(Foo), "StaticEcho", "Hello");
			Assert.AreEqual(Foo.StaticEcho("Hello"), result);
		}
	}
}
