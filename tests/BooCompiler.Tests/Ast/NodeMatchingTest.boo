namespace BooCompiler.Tests.Ast

import Boo.Lang.Compiler.Ast
import NUnit.Framework

[TestFixture]
class NodeMatchingTest:
	[Test]
	def SimpleTypeReferenceWontMatchGenericTypeReferenceWithSamePrefix():
		Assert.IsFalse(SimpleTypeReference("Foo").Matches(GenericTypeReference("Foo")))

	[Test]
	def GenericTypeReferenceMatchesGenericTypeReferenceWithSamePrefix():
		Assert.IsTrue(GenericTypeReference("Foo").Matches(GenericTypeReference("Foo")))
