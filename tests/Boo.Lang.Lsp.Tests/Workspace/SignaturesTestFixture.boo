namespace Boo.Lang.Lsp.Tests.Workspace

import System
import System.Text
import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute)
import Boo.Lang.Compiler
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Environments
import Boo.Lang.Lsp.Workspace

[TestFixture]
class SignaturesTestFixture:
"""
The word hover puts in front of a type name. Types come from the type system
rather than from source, so each kind is asked for by mapping the .NET type
that has it.
"""

	context as CompilerContext

	[SetUp]
	def Setup():
		document = TextDocument("file:///a.boo", "boo", 1, "print 'x'\n")
		context = Analyzer().Bound(document)

	private def KindOf(type as Type) as string:
		found as string
		ActiveEnvironment.With(context.Environment):
			found = Signatures.KindOf(my(TypeSystemServices).Map(type))
		return found

	[Test]
	def NamesAClass():
		assert "class" == KindOf(typeof(StringBuilder))

	[Test]
	def NamesAnInterface():
		assert "interface" == KindOf(typeof(IDisposable))

	[Test]
	def NamesAnEnum():
	"""Before struct, since an enum is a value type too."""
		assert "enum" == KindOf(typeof(DayOfWeek))

	[Test]
	def NamesAStruct():
		assert "struct" == KindOf(typeof(DateTime))

	[Test]
	def NamesACallable():
	"""The case the review asked for."""
		assert "callable" == KindOf(typeof(Action))

	[Test]
	def NamesAByrefLikeAsARefStruct():
	"""Before struct: a byreflike is a value type, and struct alone is not what the parser takes back."""
		assert "ref struct" == KindOf(typeof(Span[of int]))
