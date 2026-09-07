namespace Boo.Lang.Lsp.Tests.Workspace

import System
import System.IO
import System.Collections.Generic
import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute, TearDownAttribute, Assert)
import Boo.Lang.Lsp.Workspace

[TestFixture]
class ProjectTestFixture:
"""Finding the project a source file belongs to."""

	root as string

	[SetUp]
	def Setup():
		root = Path.Combine(Path.GetTempPath(), "boo-ls-" + Guid.NewGuid().ToString("N"))
		Directory.CreateDirectory(root)

	[TearDown]
	def Teardown():
		Directory.Delete(root, true) if Directory.Exists(root)

	private def Write(relative as string, text as string) as string:
		# Combine leaves a forward slash alone on Windows, and what the code
		# under test returns comes back with the platform's own separator.
		path = Path.Combine(root, relative.Replace(char('/'), Path.DirectorySeparatorChar))
		Directory.CreateDirectory(Path.GetDirectoryName(path))
		File.WriteAllText(path, text)
		return path

	[Test]
	def FindsAProjectBesideTheSource():
		project = Write("app/App.booproj", "<Project />")
		source = Write("app/Main.boo", "print 1")
		assert Project.Find(source) == project

	[Test]
	def FindsAProjectInAParentDirectory():
		project = Write("app/App.booproj", "<Project />")
		source = Write("app/deep/down/Main.boo", "print 1")
		assert Project.Find(source) == project

	[Test]
	def StopsAtTheNearestProject():
		Write("app/Outer.booproj", "<Project />")
		inner = Write("app/inner/Inner.booproj", "<Project />")
		source = Write("app/inner/Main.boo", "print 1")
		assert Project.Find(source) == inner

	[Test]
	def FindsNothingWhenNoProjectIsAbove():
		source = Write("loose/Main.boo", "print 1")
		assert Project.Find(source) is null

	[Test]
	def FindsNothingForAPathThatDoesNotExist():
		assert Project.Find(Path.Combine(root, "gone", "Main.boo")) is null

	private def Joined(paths as List[of string]) as string:
		return string.Join(",", paths.ToArray())

	private def Names(paths as List[of string]) as string:
		names = List[of string]()
		for path in paths:
			names.Add(Path.GetFileName(path))
		return string.Join(",", names.ToArray())

	[Test]
	def CollectsTheSourcesBesideTheProject():
		project = Write("app/App.booproj", "<Project />")
		Write("app/Main.boo", "print 1")
		Write("app/Other.boo", "print 2")
		assert Names(Project.SourceFiles(project)) == "Main.boo,Other.boo"

	[Test]
	def CollectsSourcesFromSubdirectories():
		project = Write("app/App.booproj", "<Project />")
		Write("app/deep/Nested.boo", "print 1")
		assert Names(Project.SourceFiles(project)) == "Nested.boo"

	[Test]
	def LeavesOutBuildOutput():
		project = Write("app/App.booproj", "<Project />")
		Write("app/Main.boo", "print 1")
		Write("app/bin/Debug/Copy.boo", "print 1")
		Write("app/obj/Debug/Generated.boo", "print 1")
		assert Names(Project.SourceFiles(project)) == "Main.boo"

	[Test]
	def LeavesOutFilesThatAreNotBoo():
		project = Write("app/App.booproj", "<Project />")
		Write("app/Main.boo", "print 1")
		Write("app/readme.md", "hello")
		assert Names(Project.SourceFiles(project)) == "Main.boo"

	[Test]
	def CollectsNothingForAProjectThatIsNotThere():
		assert Project.SourceFiles(Path.Combine(root, "gone", "App.booproj")).Count == 0

	[Test]
	def ReadsTheProjectsAProjectReferences():
		project = Write("app/App.booproj", """
			<Project Sdk="Microsoft.NET.Sdk">
			  <ItemGroup>
			    <ProjectReference Include="../lib/Lib.booproj" />
			    <ProjectReference Include="../core/Core.csproj" />
			  </ItemGroup>
			</Project>
			""")
		lib = Write("lib/Lib.booproj", "<Project />")
		core = Write("core/Core.csproj", "<Project />")
		assert Joined(Project.ProjectReferences(project)) == lib + "," + core

	[Test]
	def LeavesOutAReferenceThatContributesNoAssembly():
		project = Write("app/App.booproj", """
			<Project>
			  <ItemGroup>
			    <ProjectReference Include="../tool/Tool.csproj" ReferenceOutputAssembly="false" />
			  </ItemGroup>
			</Project>
			""")
		Write("tool/Tool.csproj", "<Project />")
		assert Project.ProjectReferences(project).Count == 0

	[Test]
	def AcceptsWindowsSeparatorsInAReference():
		project = Write("app/App.booproj", """
			<Project>
			  <ItemGroup><ProjectReference Include="..\\lib\\Lib.booproj" /></ItemGroup>
			</Project>
			""")
		lib = Write("lib/Lib.booproj", "<Project />")
		assert Joined(Project.ProjectReferences(project)) == lib

	[Test]
	def ReadsNoReferencesFromAProjectWithNone():
		project = Write("app/App.booproj", "<Project Sdk=\"Microsoft.NET.Sdk\" />")
		assert Project.ProjectReferences(project).Count == 0

	[Test]
	def ReadsNoReferencesFromAProjectThatWillNotParse():
		project = Write("app/App.booproj", "<Project><ItemGroup>")
		assert Project.ProjectReferences(project).Count == 0

	private def WriteAssembly(relative as string, age as int) as string:
		path = Write(relative, "not really an assembly")
		File.SetLastWriteTimeUtc(path, DateTime.UtcNow.AddMinutes(-age))
		return path

	[Test]
	def FindsWhatAProjectBuildsInto():
		project = Write("lib/Lib.booproj", "<Project />")
		assembly = WriteAssembly("lib/bin/Debug/net10.0/Lib.dll", 1)
		assert Project.OutputAssembly(project) == assembly

	[Test]
	def PrefersTheAssemblyBuiltMostRecently():
		project = Write("lib/Lib.booproj", "<Project />")
		WriteAssembly("lib/bin/Debug/net10.0/Lib.dll", 10)
		fresh = WriteAssembly("lib/bin/Release/net10.0/Lib.dll", 1)
		assert Project.OutputAssembly(project) == fresh

	[Test]
	def NamesTheAssemblyTheWayTheProjectDoes():
		project = Write("lib/Lib.booproj", "<Project><PropertyGroup><AssemblyName>Renamed</AssemblyName></PropertyGroup></Project>")
		WriteAssembly("lib/bin/Debug/net10.0/Lib.dll", 1)
		renamed = WriteAssembly("lib/bin/Debug/net10.0/Renamed.dll", 1)
		assert Project.OutputAssembly(project) == renamed

	[Test]
	def FindsNothingForAProjectThatWasNeverBuilt():
		project = Write("lib/Lib.booproj", "<Project />")
		assert Project.OutputAssembly(project) is null

	[Test]
	def FindsNothingForAProjectFileThatIsMissing():
		assert Project.OutputAssembly(Path.Combine(root, "gone", "Lib.booproj")) is null

	[Test]
	def ReferencesWhatSitsBesideTheBuiltAssembly():
		project = Write("app/App.booproj", "<Project />")
		WriteAssembly("app/bin/Debug/net10.0/App.dll", 1)
		WriteAssembly("app/bin/Debug/net10.0/Lib.dll", 1)
		WriteAssembly("app/bin/Debug/net10.0/Other.dll", 1)
		assert Names(Project.References(project)) == "Lib.dll,Other.dll"

	[Test]
	def ReferencesTheNewestBuildOnly():
		project = Write("app/App.booproj", "<Project />")
		WriteAssembly("app/bin/Debug/net10.0/App.dll", 10)
		WriteAssembly("app/bin/Debug/net10.0/Stale.dll", 10)
		WriteAssembly("app/bin/Release/net10.0/App.dll", 1)
		WriteAssembly("app/bin/Release/net10.0/Fresh.dll", 1)
		assert Names(Project.References(project)) == "Fresh.dll"

	[Test]
	def ReferencesNothingFromAProjectThatWasNeverBuilt():
		project = Write("app/App.booproj", "<Project />")
		assert Project.References(project).Count == 0

	private def WriteAssets(compile as string):
	"""An assets file naming one package, restored under the temp root."""
		folder = Path.Combine(root, "packages").Replace("\\", "\\\\")
		Write("app/obj/project.assets.json", """
		{
		  "version": 3,
		  "targets": {
		    "net10.0": {
		      "Fake/1.0.0": { "type": "package", "compile": { "${compile}": {} } },
		      "Boo.Lang/1.0.0": { "type": "project", "compile": { "bin/placeholder/Boo.Lang.dll": {} } }
		    }
		  },
		  "libraries": { "Fake/1.0.0": { "type": "package", "path": "fake/1.0.0" } },
		  "packageFolders": { "${folder}": {} }
		}
		""")

	[Test]
	def ReferencesThePackagesTheProjectRestored():
		project = Write("app/App.booproj", "<Project />")
		WriteAssembly("app/bin/Debug/net10.0/App.dll", 1)
		WriteAssembly("packages/fake/1.0.0/lib/net10.0/Fake.dll", 1)
		WriteAssets("lib/net10.0/Fake.dll")
		assert Names(Project.References(project)) == "Fake.dll"

	[Test]
	def ReferencesWhatAReferencedProjectBuilt():
		project = Write("app/App.booproj", """
			<Project>
			  <ItemGroup><ProjectReference Include="../lib/Lib.booproj" /></ItemGroup>
			</Project>
			""")
		Write("lib/Lib.booproj", "<Project />")
		WriteAssembly("lib/bin/Debug/net10.0/Lib.dll", 1)
		assert Names(Project.References(project)) == "Lib.dll"

	[Test]
	def PrefersTheCopyBesideItsOwnOutput():
		project = Write("app/App.booproj", """
			<Project>
			  <ItemGroup><ProjectReference Include="../lib/Lib.booproj" /></ItemGroup>
			</Project>
			""")
		Write("lib/Lib.booproj", "<Project />")
		WriteAssembly("lib/bin/Debug/net10.0/Lib.dll", 1)
		WriteAssembly("app/bin/Debug/net10.0/App.dll", 1)
		beside = WriteAssembly("app/bin/Debug/net10.0/Lib.dll", 1)
		assert Joined(Project.References(project)) == beside

	[Test]
	def ReferencesThePackagesOfAProjectThatWasNeverBuilt():
		project = Write("app/App.booproj", "<Project />")
		WriteAssembly("packages/fake/1.0.0/lib/net10.0/Fake.dll", 1)
		WriteAssets("lib/net10.0/Fake.dll")
		assert Names(Project.References(project)) == "Fake.dll"

	[Test]
	def ReferencesNothingForAProjectFileThatIsMissing():
		assert Project.References(Path.Combine(root, "gone", "App.booproj")).Count == 0

	[Test]
	def LeavesOutAPackageThatCarriesNothingToCompileAgainst():
		project = Write("app/App.booproj", "<Project />")
		WriteAssembly("app/bin/Debug/net10.0/App.dll", 1)
		WriteAssets("lib/net10.0/_._")
		assert Project.References(project).Count == 0

	[Test]
	def LeavesOutAPackageThatIsNotOnDisk():
		project = Write("app/App.booproj", "<Project />")
		WriteAssembly("app/bin/Debug/net10.0/App.dll", 1)
		WriteAssets("lib/net10.0/Fake.dll")
		assert Project.References(project).Count == 0

	[Test]
	def FindsTheProjectBehindADocumentUri():
		project = Write("app/App.booproj", "<Project />")
		source = Write("app/Main.boo", "print 1")
		assert Project.FindForDocument(Uri(source).AbsoluteUri) == project

	[Test]
	def FindsTheProjectBehindAUriWithAnEscapedPath():
		project = Write("app/my app/App.booproj", "<Project />")
		source = Write("app/my app/Main.boo", "print 1")
		assert Project.FindForDocument(Uri(source).AbsoluteUri) == project

	[Test]
	def TurnsAPathIntoAUri():
		path = Path.Combine(root, "app", "Main.boo")
		assert Project.UriOf(path) == Uri(path).AbsoluteUri

	[Test]
	def LeavesAUriAsItIs():
		uri = Uri(Path.Combine(root, "app", "Main.boo")).AbsoluteUri
		assert Project.UriOf(uri) == uri

	[Test]
	def LeavesSomethingThatNamesNoFileAsItIs():
		assert Project.UriOf("untitled:Untitled-1") == "untitled:Untitled-1"

	[Test]
	def FindsNoProjectForADocumentThatIsNotAFile():
		assert Project.FindForDocument("untitled:Untitled-1") is null

	[Test]
	def FindsNoProjectForAUriThatIsNotOne():
		assert Project.FindForDocument("not a uri at all") is null

	private def WriteAssetsWithRuntime(compile as string, runtime as string):
	"""An assets file for a package that ships a reference assembly as well."""
		folder = Path.Combine(root, "packages").Replace("\\", "\\\\")
		Write("app/obj/project.assets.json", """
		{
		  "version": 3,
		  "targets": {
		    "net10.0": {
		      "Fake/1.0.0": {
		        "type": "package",
		        "compile": { "${compile}": {} },
		        "runtime": { "${runtime}": {} }
		      }
		    }
		  },
		  "libraries": { "Fake/1.0.0": { "type": "package", "path": "fake/1.0.0" } },
		  "packageFolders": { "${folder}": {} }
		}
		""")

	[Test]
	def PrefersTheRuntimeAssemblyToTheReferenceAssembly():
	"""Assembly.LoadFrom refuses a reference assembly, so the ref/ copy is no use."""
		project = Write("app/App.booproj", "<Project />")
		WriteAssembly("app/bin/Debug/net10.0/App.dll", 1)
		WriteAssembly("packages/fake/1.0.0/ref/net10.0/Fake.dll", 1)
		lib = WriteAssembly("packages/fake/1.0.0/lib/net10.0/Fake.dll", 1)
		WriteAssetsWithRuntime("ref/net10.0/Fake.dll", "lib/net10.0/Fake.dll")
		assert Joined(Project.References(project)) == lib

	[Test]
	def FallsBackToTheReferenceAssemblyWhenThereIsNoRuntimeOne():
		project = Write("app/App.booproj", "<Project />")
		WriteAssembly("app/bin/Debug/net10.0/App.dll", 1)
		reference = WriteAssembly("packages/fake/1.0.0/ref/net10.0/Fake.dll", 1)
		WriteAssets("ref/net10.0/Fake.dll")
		assert Joined(Project.References(project)) == reference

	[Test]
	def TakesOnlySoManyFilesFromABigDirectory():
	"""
	A directory of hundreds compiled as one program is slow, and the
	compiler's name resolution gives out somewhere past a few hundred.
	"""
		source = Write("loose/Main.boo", "print 1\n")
		for i in range(Project.SiblingLimit + 40):
			Write("loose/module${i}.boo", "def helper${i}() as int:\n\treturn ${i}\n")
		assert Project.LooseSiblings(source, "print 1\n").Count == Project.SiblingLimit

	[Test]
	def KeepsWhatAnImportNamesAheadOfWhatMerelySitsThere():
	"""An import says the file is wanted; being in the same directory guesses it."""
		source = Write("mixed/Main.boo", "import Wanted\n\nprint 1\n")
		asked = Write("mixed/wanted.boo", "namespace Wanted\n\nclass Thing:\n\tpass\n")
		for i in range(Project.SiblingLimit + 10):
			Write("mixed/module${i}.boo", "def helper${i}() as int:\n\treturn ${i}\n")
		siblings = Project.LooseSiblings(source, "import Wanted\n\nprint 1\n")
		assert siblings.Count == Project.SiblingLimit
		assert siblings.Contains(asked), "the imported file was crowded out"
