namespace Boo.Lang.Lsp.Tests.Server

import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Server

[TestFixture]
class CommandLineOptionsTestFixture:

	private def Parse(*args as (string)):
		return CommandLineOptions.Parse(args)

	[Test]
	def ServesWhenGivenNothing():
		assert Parse().Action == CommandLineOptions.Serve

	[Test]
	def ServesWhenToldToUseStdio():
		# Every LSP client built on vscode-languageclient appends --stdio.
		assert Parse("--stdio").Action == CommandLineOptions.Serve

	[Test]
	def AcceptsTheOtherSpellingsOfStdio():
		assert Parse("-stdio").Action == CommandLineOptions.Serve

	[Test]
	def ShowsTheVersion():
		assert Parse("--version").Action == CommandLineOptions.ShowVersion
		assert Parse("-version").Action == CommandLineOptions.ShowVersion

	[Test]
	def ShowsTheHelp():
		assert Parse("--help").Action == CommandLineOptions.ShowHelp
		assert Parse("-h").Action == CommandLineOptions.ShowHelp

	[Test]
	def RefusesAnOptionItDoesNotKnow():
		parsed = Parse("--nonsense")
		assert parsed.Action == CommandLineOptions.Unknown
		assert parsed.UnknownOption == "--nonsense"

	[Test]
	def TakesTheFirstInstructionItIsGiven():
		assert Parse("--version", "--help").Action == CommandLineOptions.ShowVersion

	[Test]
	def StillServesWhenStdioComesAlongsideNothingElse():
		assert Parse("--stdio", "--stdio").Action == CommandLineOptions.Serve

	[Test]
	def RefusesAValueGivenToAFlag():
	"""A flag takes no value, and the parser is what notices."""
		assert Parse("--stdio=nonsense").Action == CommandLineOptions.Unknown

	[Test]
	def RefusesSomethingThatIsNotAnOptionAtAll():
		parsed = Parse("wat")
		assert parsed.Action == CommandLineOptions.Unknown
		assert parsed.UnknownOption == "wat"

	[Test]
	def ServesWhenNothingIsAsked():
		assert Parse().Action == CommandLineOptions.Serve

	[Test]
	def TakesTheHelpWhenItComesFirst():
		assert Parse("--help", "--version").Action == CommandLineOptions.ShowHelp

	[Test]
	def NamesTheOptionItCouldNotPlace():
		assert Parse("--stdio", "--nope").UnknownOption == "--nope"
