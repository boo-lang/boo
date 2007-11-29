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
			set { _name = value; }
		}

		private static string _zeng = "Static property";

		public static string Zeng
		{
			get { return _zeng;  }
			set { _zeng = value; }
		}
	}

	[TestFixture]
	public class PropertyDispatcherFactoryTestCase : AbstractDispatcherFactoryTestCase
	{
		[Test]
		public void InstanceProperty()
		{
			Bar o = new Bar("John Cleese");
			Assert.AreEqual(o.Name, Get(o, "Name"));
		}

		[Test]
		public void StaticProperty()
		{
			Assert.AreEqual(Bar.Zeng, Get(new Bar("foo"), "Zeng"));
		}

		[Test]
		public void ExtensionProperty()
		{
			// c# does not allow indexed properties
			string code = @"
class ArrayExtensions:

	[Extension]
	static length[a as System.Array]:
		get:
			return a.Length

class StringExtensions:
			
	[Extension]
	static length[s as string]:
		get:
			return s.Length
";
			Assembly assembly = compile(code);
			_extensions.Register(assembly.GetType("ArrayExtensions"));
			_extensions.Register(assembly.GetType("StringExtensions"));

			string name = "Eric Idle";
			Assert.AreEqual(name.Length, Get(name, "length"));
			Assert.AreEqual(name.Length, Get(name.ToCharArray(), "length"));
		}

		private Assembly compile(string code)
		{
			return Compilation.compile(BooParser.ParseString("code", code), GetType().Assembly);
		}

		[Test]
		public void InstanceField()
		{
			Bar o = new Bar("John Cleese");
			Assert.AreEqual(o.Name, Get(o, "_name"));
		}

		[Test]
		public void StaticField()
		{
			Assert.AreEqual(Bar.Zeng, Get(new Bar("foo"), "_zeng"));
			Assert.AreEqual(Bar.Zeng, Get(null, typeof(Bar), "_zeng"));
		}

		[Test]
		public void SetInstanceProperty()
		{
			AssertSetName("Name");
		}

		[Test]
		public void SetInstanceField()
		{
			AssertSetName("_name");
		}
		
		[Test]
		public void SetStaticField()
		{
			string expected = "42";
			Assert.AreEqual(expected, Set(new Bar("foo"), "_zeng", expected));
			Assert.AreEqual(expected, Bar.Zeng);

			expected = "75";
			Assert.AreEqual(expected, Set(null, typeof(Bar), "_zeng", expected));
			Assert.AreEqual(expected, Bar.Zeng);
		}

		private void AssertSetName(string name)
		{
			Bar o = new Bar("John Cleese");
			string expected = "Eric Idle";
			object value = Set(o, name, expected);
			Assert.AreEqual(expected, value);
			Assert.AreEqual(expected, o.Name);
		}

		private object Set(object o, string name, object value)
		{
			return Set(o, o.GetType(), name, value);
		}

		private object Set(object o, Type type, string name, object value)
		{
			Dispatcher dispatcher = new PropertyDispatcherFactory(_extensions, o, type, name, value).CreateSetter();
			return dispatcher(o, new object[] { value });
		}

		private object Get(object o, string name)
		{
			return Get(o, o.GetType(), name);
		}

		private object Get(object o, Type type, string name)
		{
			Dispatcher dispatcher = new PropertyDispatcherFactory(_extensions, o, type, name).CreateGetter();
			return dispatcher(o, null);
		}
	}
}
