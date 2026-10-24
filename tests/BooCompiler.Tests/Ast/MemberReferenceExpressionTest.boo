namespace BooCompiler.Tests.Ast

import Boo.Lang.Compiler.Ast
import NUnit.Framework

[TestFixture]
class MemberReferenceExpressionTest:

	[Test]
	def CombineWithAName():
		result = MemberReferenceExpression.Combine(ReferenceExpression("A"), ReferenceExpression("b"))
		Assert.AreEqual("A.b", result.ToCodeString())

	[Test]
	def CombineKeepsTheQualifiersOfAQualifiedMember():
		member = MemberReferenceExpression(ReferenceExpression("B"), "c")
		result = MemberReferenceExpression.Combine(ReferenceExpression("A"), member)
		Assert.AreEqual("A.B.c", result.ToCodeString())

	[Test]
	def CombineOneLevelAtATime():
		# How a method's name is built up through the types that contain it.
		name as ReferenceExpression = ReferenceExpression("m")
		for owner in ("Inner", "Middle", "Outer"):
			name = MemberReferenceExpression.Combine(ReferenceExpression(owner), name)
		Assert.AreEqual("Outer.Middle.Inner.m", name.ToCodeString())

	[Test]
	def CombineWithAChainOfCallsAndIndexers():
		member = Boo.Lang.Parser.BooParser.ParseExpression("test", "myVar.Foo().Bar[3].Baz") as MemberReferenceExpression
		result = MemberReferenceExpression.Combine(ReferenceExpression("A"), member)
		Assert.AreEqual("A.myVar.Foo().Bar[3].Baz", result.ToCodeString())

	[Test]
	def CombineWithAGenericReference():
		member = Boo.Lang.Parser.BooParser.ParseExpression("test", "myVar.Foo[of int]().Baz") as MemberReferenceExpression
		result = MemberReferenceExpression.Combine(ReferenceExpression("A"), member)
		Assert.AreEqual("A.myVar.Foo[of int]().Baz", result.ToCodeString())

	[Test]
	def CombineLeavesTheMemberItWasGivenAlone():
		member = Boo.Lang.Parser.BooParser.ParseExpression("test", "myVar.Foo().Baz") as MemberReferenceExpression
		MemberReferenceExpression.Combine(ReferenceExpression("A"), member)
		Assert.AreEqual("myVar.Foo().Baz", member.ToCodeString())

	[Test]
	def CombineRefusesAnExpressionThatDoesNotStartWithAName():
		member = Boo.Lang.Parser.BooParser.ParseExpression("test", "'text'.Length") as MemberReferenceExpression
		Assert.Throws[of Boo.Lang.Compiler.CompilerError]() do:
			MemberReferenceExpression.Combine(ReferenceExpression("A"), member)

	[Test]
	def CombineKeepsNamedArguments():
		member = Boo.Lang.Parser.BooParser.ParseExpression("test", "b.Foo(1, x: 2).Baz") as MemberReferenceExpression
		result = MemberReferenceExpression.Combine(ReferenceExpression("A"), member)
		Assert.AreEqual("A.b.Foo(1, x: 2).Baz", result.ToCodeString())

	[Test]
	def CombineLeavesTheArgumentsAndIndicesOfTheOriginalInPlace():
		member = Boo.Lang.Parser.BooParser.ParseExpression("test", "b.Foo(1, x: 2).Bar[3, 4].Baz") as MemberReferenceExpression
		MemberReferenceExpression.Combine(ReferenceExpression("A"), member)
		slice = member.Target as SlicingExpression
		call = (slice.Target as MemberReferenceExpression).Target as MethodInvocationExpression
		for index in slice.Indices:
			Assert.AreSame(slice, index.ParentNode)
		Assert.AreSame(call, call.Arguments[0].ParentNode)
		Assert.AreSame(call, call.NamedArguments[0].ParentNode)
