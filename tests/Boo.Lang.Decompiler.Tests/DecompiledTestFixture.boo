namespace Boo.Lang.Decompiler.Tests

import System
import System.IO
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Parser
import Boo.Lang.Decompiler

[TestFixture]
class DecompiledTestFixture:
"""Reading a type out of an assembly and writing it as Boo."""

	private static final CoreLib = typeof(System.IO.Path).Assembly.Location

	private def Written(fullName as string) as string:
		text = Decompiled.Of(CoreLib, fullName)
		assert text is not null, "nothing written for ${fullName}"
		return text

	[Test]
	def WritesTheNamespaceAndTheType():
		written = Written("System.IO.Path")
		assert written.StartsWith("namespace System.IO\n"), written
		assert "class Path:" in written, written

	[Test]
	def WritesAMemberSignature():
		assert "static def GetTempPath() as string:" in Written("System.IO.Path")

	[Test]
	def WritesAnArrayAndAGenericTheBooWay():
		assert "as (char)" in Written("System.IO.Path")
		assert "class List[of T]:" in Written("System.Collections.Generic.List`1")

	[Test]
	def WritesABodyRatherThanPass():
		written = Written("System.IO.Path")
		assert "if path == null:" in written, "no converted statement"
		assert "while num2 >= 0:" in written, "no converted loop"

	[Test]
	def NamesWhatItCannotWrite():
		assert "# TODO:" in Written("System.IO.Path")

	[Test]
	def SaysNothingForATypeTheAssemblyLacks():
		assert Decompiled.Of(CoreLib, "No.Such.Type") is null

	private def Across(*names as (string)) as string:
		written = ""
		for name in names:
			written += Written(name)
		return written

	[Test]
	def WritesTheStatementsItConverts():
	"""
	Read off a handful of types rather than one method, so a change in
	what the framework ships does not fail the wrong thing.
	"""
		written = Across("System.IO.Path", "System.Convert", "System.TimeSpan")
		assert "elif " in written, "no switch converted"
		assert "try:" in written, "no try converted"
		assert "except " in written, "no catch converted"
		assert "for " in written, "no foreach converted"

	[Test]
	def WritesTheExpressionsItConverts():
		written = Across("System.IO.Path", "System.Convert", "System.Version")
		assert "+=" in written, "no compound assignment"
		assert "cast(" in written, "no cast"
		assert "raise " in written, "no throw"

	[Test]
	def WritesBooTheParserAccepts():
		for name in ("System.IO.Path", "System.Collections.Generic.List`1", "System.Version"):
			path = Path.Combine(Path.GetTempPath(), "dec-" + Guid.NewGuid().ToString("N") + ".boo")
			File.WriteAllText(path, Written(name))
			try:
				BooParser.ParseFile(path)
			ensure:
				File.Delete(path)

	[Test]
	def WritesCSharpWhenAskedFor():
		written = Decompiled.AsCSharp(CoreLib, "System.IO.Path")
		assert written is not null, "nothing written"
		assert "public static class Path" in written, written.Split(char('\n'))[0]
		assert "def " not in written, "that is Boo, not C#"

	[Test]
	def SaysNothingInCSharpForATypeTheAssemblyLacks():
		assert Decompiled.AsCSharp(CoreLib, "No.Such.Type") is null
