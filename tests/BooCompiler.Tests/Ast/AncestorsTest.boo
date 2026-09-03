namespace BooCompiler.Tests.Ast

import System.Linq
import Boo.Lang.Compiler.Ast
import NUnit.Framework

[TestFixture]
class AncestorsTest:
	[Test]
	def NoAncestorsForUnparentedNode():
		Assert.AreEqual(0, SimpleTypeReference().GetAncestors[of Node]().Count())
