namespace BooCompiler.Tests

import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import NUnit.Framework

[TestFixture]
class EndKeywordTest:
"""
`end` is optional outside whitespace agnostic mode, and closes nothing.

The layout still decides where a block ends, so a lone `end` is dropped and
has to line up the way any other line would. Anywhere a name could stand
instead, a name is what it is.

The sources are written flush left: indentation is the syntax under test, and
escaping it reads as nothing at all.
"""

	private def BoundErrors(code as string):
		compiler = BooCompiler()
		compiler.Parameters.Input.Add(StringInput("testcase", code))
		compiler.Parameters.Pipeline = ResolveExpressions(BreakOnErrors: false)
		return compiler.Run().Errors

	[Test]
	def DropsALoneEndAfterABlockDedents():
		Assert.AreEqual(0, BoundErrors("""
def f() as int:
	return 1
end

print f()
""").Count)

	[Test]
	def DropsALoneEndAfterEachNestedBlock():
		Assert.AreEqual(0, BoundErrors("""
class C:
	def g() as int:
		return 1
	end
end

print C().g()
""").Count)

	[Test]
	def KeepsEndAsAnIdentifier():
		Assert.AreEqual(0, BoundErrors("""
def f() as int:
	end = 1
	return end

print f()
""").Count)

	[Test]
	def KeepsEndAsAnIdentifierOnALineThatDedents():
	"""A dedent alone is not enough to drop it: nothing may follow it."""
		Assert.AreEqual(0, BoundErrors("""
def f() as int:
	if 1 > 0:
		pass
	end = 1
	return end

print f()
""").Count)

	[Test]
	def RefusesALoneEndThatDedentsNothing():
	"""Lining up with the block it sits in leaves it a name, and an unknown one."""
		Assert.AreEqual(1, BoundErrors("""
def f():
	pass
	end
""").Count)
