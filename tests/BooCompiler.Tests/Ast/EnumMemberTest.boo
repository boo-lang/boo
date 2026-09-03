namespace BooCompiler.Tests.Ast

import Boo.Lang.Compiler.Ast
import NUnit.Framework

[TestFixture]
class EnumMemberTest:
	[Test]
	def EnumMemberIsStatic():
		Assert.IsTrue(EnumMember().IsStatic)
