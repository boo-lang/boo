namespace Boo.Lang.Lsp.Tests.Server

import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Server

[TestFixture]
class CommandLineTestFixture:

	private def Parse(*args as (string)):
		return CommandLine.Parse(args)

	[Test]
	def ServesWhenGivenNothing():
		assert Parse().Action == CommandLine.Serve

	[Test]
	def ServesWhenToldToUseStdio():
		# Every LSP client built on vscode-languageclient appends --stdio, and
		# a server that refuses it exits before it has said anything.
		assert Parse("--stdio").Action == CommandLine.Serve

	[Test]
	def AcceptsTheOtherSpellingsOfStdio():
		assert Parse("-stdio").Action == CommandLine.Serve

	[Test]
	def ShowsTheVersion():
		assert Parse("--version").Action == CommandLine.ShowVersion
		assert Parse("-version").Action == CommandLine.ShowVersion

	[Test]
	def ShowsTheHelp():
		assert Parse("--help").Action == CommandLine.ShowHelp
		assert Parse("-h").Action == CommandLine.ShowHelp

	[Test]
	def RefusesAnOptionItDoesNotKnow():
		parsed = Parse("--nonsense")
		assert parsed.Action == CommandLine.Unknown
		assert parsed.UnknownOption == "--nonsense"

	[Test]
	def TakesTheFirstInstructionItIsGiven():
		assert Parse("--version", "--help").Action == CommandLine.ShowVersion

	[Test]
	def StillServesWhenStdioComesAlongsideNothingElse():
		assert Parse("--stdio", "--stdio").Action == CommandLine.Serve
