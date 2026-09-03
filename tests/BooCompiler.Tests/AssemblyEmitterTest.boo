namespace BooCompiler.Tests

import System.Reflection
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming
import NUnit.Framework

[TestFixture]
class AssemblyEmitterTest:
	[Test]
	[Ignore("work in progress")]
	def VirtualMethodsAreTaggedNewSlot():
		classDefinition = ClassDefinition(Name: "Foo")
		classDefinition.Members.Add(
			Method(Name: "Bar", Modifiers: TypeMemberModifiers.Virtual))

		type = Compilation.compile(classDefinition)
		method = type.GetMethod("Bar")
		Assert.AreEqual(
			MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.HideBySig,
			method.Attributes)
