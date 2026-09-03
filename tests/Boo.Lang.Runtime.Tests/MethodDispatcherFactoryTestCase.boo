namespace Boo.Lang.Runtime.Tests

import System
import Boo.Lang.Runtime.DynamicDispatching
import NUnit.Framework

public class Foo:
	public def Echo(value as string) as string:
		return "Echo: " + value

	public static def StaticEcho(value as string) as string:
		return "StaticEcho: " + value

public static class FooExtensions:
	[Extension]
	public def ExtensionEcho(target as Foo, value as string) as string:
		return "ExtensionEcho: " + target.Echo(value)

	[Extension]
	public def ExtensionEchoVar(target as Foo, i as int, *value as (string)) as string:
		return "ExtensionEchoVar(" + i + ", " + string.Join(", ", value) + ")"

[TestFixture]
class MethodDispatcherFactoryTestCase(AbstractDispatcherFactoryTestCase):
	[Test]
	def ExtensionMethod():
		o = Foo()
		value = Dispatch(o, "ExtensionEcho", ("Hello",))
		Assert.AreEqual(FooExtensions.ExtensionEcho(o, "Hello"), value)

	[Test]
	def ExtensionMethodWithVarArgs():
		o = Foo()
		value = Dispatch(o, "ExtensionEchoVar", (1, "skip", "Hello"))
		Assert.AreEqual(FooExtensions.ExtensionEchoVar(o, 1, "skip", "Hello"), value)

	[Test]
	def InstanceMethod():
		o = Foo()
		value = Dispatch(o, "Echo", ("Hello",))
		Assert.AreEqual(o.Echo("Hello"), value)

	def Dispatch(o as Foo, methodName as string, args as (object)) as object:
		return Dispatch(o, o.GetType(), methodName, args)

	def Dispatch(o as Foo, type as Type, methodName as string, args as (object)) as object:
		return MethodDispatcherFactory(_extensions, o, type, methodName, *args).Create()(o, args)

	[Test]
	def StaticMethod():
		AssertStaticMethod(Foo())
		AssertStaticMethod(null)
		AssertStaticMethod(Foo())

	def AssertStaticMethod(target as Foo):
		result = Dispatch(target, Foo, "StaticEcho", ("Hello",))
		Assert.AreEqual(Foo.StaticEcho("Hello"), result)
