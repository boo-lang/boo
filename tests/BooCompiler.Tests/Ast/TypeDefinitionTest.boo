namespace BooCompiler.Tests.Ast

import Boo.Lang.Compiler.Ast
import NUnit.Framework

[TestFixture]
class TypeDefinitionTest:
	[Test]
	def MergeIgnoresMatchingBaseTypes():
		foo = SimpleTypeReference("Foo")
		bar = SimpleTypeReference("Bar")
		fooOfBar = GenericTypeReference("Foo", bar)
		barOfFoo = GenericTypeReference("Bar", foo)

		subject = ClassDefinition()
		subject.BaseTypes.Add(bar)
		subject.BaseTypes.Add(fooOfBar)

		node = ClassDefinition()
		node.BaseTypes.Add(foo)
		node.BaseTypes.Add(bar.CloneNode())
		node.BaseTypes.Add(fooOfBar.CloneNode())
		node.BaseTypes.Add(barOfFoo)

		subject.Merge(node)

		Assert.AreEqual((bar, fooOfBar, foo, barOfFoo), subject.BaseTypes.ToArray())
