namespace BooCompiler.Tests.Ast

import Boo.Lang.Compiler.Ast
import NUnit.Framework

[TestFixture]
class CloningTest:
	[Test]
	def CloningShouldPreserveIsSynthetic():
		for flag in (true, false):
			node = LabelStatement(IsSynthetic: flag)
			Assert.AreEqual(flag, node.CloneNode().IsSynthetic)
