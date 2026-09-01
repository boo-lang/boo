namespace Boo.Lang.Runtime.Tests

import System
import System.Reflection
import Boo.Lang.Compiler.MetaProgramming
import Boo.Lang.Parser
import Boo.Lang.Runtime.DynamicDispatching
import NUnit.Framework

public class Bar:
	_name as string
	static _zeng = "Static property"

	def constructor(name as string):
		_name = name

	Name as string:
		get: return _name
		set: _name = value

	static Zeng as string:
		get: return _zeng
		set: _zeng = value

[TestFixture]
class PropertyDispatcherFactoryTestCase(AbstractDispatcherFactoryTestCase):
	[Test]
	def InstanceProperty():
		o = Bar("John Cleese")
		Assert.AreEqual(o.Name, Get(o, "Name"))

	[Test]
	def StaticProperty():
		Assert.AreEqual(Bar.Zeng, Get(Bar("foo"), "Zeng"))

	[Test]
	def ExtensionProperty():
		// c# does not allow indexed properties
		code = """
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
"""
		assembly = compile(code)
		_extensions.Register(assembly.GetType("ArrayExtensions"))
		_extensions.Register(assembly.GetType("StringExtensions"))

		name = "Eric Idle"
		Assert.AreEqual(name.Length, Get(name, "length"))
		Assert.AreEqual(name.Length, Get(name.ToCharArray(), "length"))

	def compile(code as string) as Assembly:
		return Compilation.compile(BooParser.ParseString("code", code), GetType().Assembly)

	[Test]
	def InstanceField():
		o = Bar("John Cleese")
		Assert.AreEqual(o.Name, Get(o, "_name"))

	[Test]
	def StaticField():
		Assert.AreEqual(Bar.Zeng, Get(Bar("foo"), "_zeng"))
		Assert.AreEqual(Bar.Zeng, Get(null, Bar, "_zeng"))

	[Test]
	def EnumFromInstance():
		Assert.AreEqual(BindingFlags.Static, Get(BindingFlags.Public, "Static"))

	[Test]
	def LiteralFromInstance():
		Assert.IsTrue(typeof(ClassWithLiteralField).GetField("Literal").IsLiteral)
		Assert.AreEqual(42, Get(ClassWithLiteralField(), "Literal"))

	class ClassWithLiteralField:
		public static final Literal = 42

	[Test]
	def SetInstanceProperty():
		AssertSetName("Name")

	[Test]
	def SetInstanceField():
		AssertSetName("_name")

	[Test]
	def SetStaticField():
		expected = "42"
		Assert.AreEqual(expected, Set(Bar("foo"), "_zeng", expected))
		Assert.AreEqual(expected, Bar.Zeng)

		expected = "75"
		Assert.AreEqual(expected, Set(null, Bar, "_zeng", expected))
		Assert.AreEqual(expected, Bar.Zeng)

	def AssertSetName(name as string):
		o = Bar("John Cleese")
		expected = "Eric Idle"
		value = Set(o, name, expected)
		Assert.AreEqual(expected, value)
		Assert.AreEqual(expected, o.Name)

	def Set(o as object, name as string, value as object) as object:
		return Set(o, o.GetType(), name, value)

	def Set(o as object, type as Type, name as string, value as object) as object:
		dispatcher = PropertyDispatcherFactory(_extensions, o, type, name, value).CreateSetter()
		return dispatcher(o, (value,))

	def Get(o as object, name as string) as object:
		return Get(o, o.GetType(), name)

	def Get(o as object, type as Type, name as string) as object:
		dispatcher = PropertyDispatcherFactory(_extensions, o, type, name).CreateGetter()
		return dispatcher(o, null)
