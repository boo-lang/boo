using System;
using System.Reflection;
using Boo.Lang.Compiler.MetaProgramming;
using Boo.Lang.Parser;
using NUnit.Framework;

namespace Boo.Lang.Runtime.Tests
{
	public class Bar
	{
		private string _name;

		public Bar(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}

		private static string _zeng = "Static property";

		public static string Zeng
		{
			get { return _zeng;  }
		}
	}

	[TestFixture]
	public class PropertyDispatcherFactoryTestCase : AbstractDispatcherFactoryTestCase
	{
		[Test]
		public void InstanceProperty()
		{
			Bar o = new Bar("John Cleese");
			Assert.AreEqual(o.Name, Dispatch(o, "Name"));
		}

		[Test]
		public void StaticProperty()
		{
			Assert.AreEqual(Bar.Zeng, Dispatch(new Bar("foo"), "Zeng"));
		}

		[Test]
		public void ExtensionProperty()
		{
			// c# does not allow indexed properties
			string code = @"
class Extensions:
	[extension]
	static DaName[b as Boo.Lang.Runtime.Tests.Bar]:
		get:
			return b.Name
";
			_extensions.Register(compile(code).GetType("Extensions"));

			Bar b = new Bar("Eric Idle");
			Assert.AreEqual(b.Name, Dispatch(b, "DaName"));
		}

		private Assembly compile(string code)
		{
			return Compilation.compile(BooParser.ParseString("code", code), GetType().Assembly);
		}

		[Test]
		public void InstanceField()
		{
			Bar o = new Bar("John Cleese");
			Assert.AreEqual(o.Name, Dispatch(o, "_name"));
		}

		[Test]
		public void StaticField()
		{
			Assert.AreEqual(Bar.Zeng, Dispatch(new Bar("foo"), "_zeng"));
			Assert.AreEqual(Bar.Zeng, Dispatch(null, typeof(Bar), "_zeng"));
		}

		private object Dispatch(object o, string name)
		{
			return Dispatch(o, o.GetType(), name);
		}

		private object Dispatch(object o, Type type, string name)
		{
			PropertyDispatcherFactory factory = new PropertyDispatcherFactory(_extensions, o, type, name);
			return factory.Create()(o, null);
		}
	}
}
