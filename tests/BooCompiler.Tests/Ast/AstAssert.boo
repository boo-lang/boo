namespace BooCompiler.Tests.Ast

import Boo.Lang.Compiler.Ast
import NUnit.Framework

static class AstAssert:
	public def Matches(expected as Node, actual as Node):
		return if expected.Matches(actual)

		Assert.Fail(string.Format("{0}('{1}') !~ {2}('{3}')", expected.GetType(), expected.ToCodeString(), actual.GetType(), actual.ToCodeString()))
