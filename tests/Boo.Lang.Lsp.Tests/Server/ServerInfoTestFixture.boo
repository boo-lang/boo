namespace Boo.Lang.Lsp.Tests.Server

import System.IO
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Server

[TestFixture]
class ServerInfoTestFixture:

	[Test]
	def NameIsTheExecutableName():
		assert ServerInfo.Name == "boo-ls"

	[Test]
	def VersionComesFromTheAssembly():
		assert ServerInfo.Version is not null
		assert ServerInfo.Version.Length > 0

	[Test]
	def BuildNamesWhereItIsRunningFrom():
	"""Which of several checkouts answered is the thing a log has to settle."""
		assert ServerInfo.Build.Contains(Path.DirectorySeparatorChar.ToString())
		assert ServerInfo.Build.Contains("code built")

	[Test]
	def BannerCarriesNameVersionAndBuild():
		assert ServerInfo.Banner.Contains(ServerInfo.Name)
		assert ServerInfo.Banner.Contains(ServerInfo.Version)
		assert ServerInfo.Banner.Contains(ServerInfo.Build)
