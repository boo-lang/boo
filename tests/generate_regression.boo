#!env booi
#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

import System
import System.IO
import System.Linq
import Boo.Lang.Compiler.Ast.Visitors
import Boo.Lang.PatternMatching
import Boo.Lang.Parser

def Main(argv as (string)):
	
	print "{0,-5} {1,-7}  {2}" % ('tests', 'ignored', 'directory')
	
	GenerateIntegrationTestFixtures()
	
	GenerateTestFixture("regression", "BooCompiler.Tests/RegressionTestFixture.generated.boo", "BooCompiler.Regression", """
	namespace BooCompiler.Tests

	import NUnit.Framework

	[TestFixture]
	class RegressionTestFixture(AbstractCompilerTestCase):
	""")
	
	GenerateTestFixture("errors", "BooCompiler.Tests/CompilerErrorsTestFixture.generated.boo", "BooCompiler.CompilerErrors", """
	namespace BooCompiler.Tests

	import NUnit.Framework

	[TestFixture]
	class CompilerErrorsTestFixture(AbstractCompilerErrorsTestFixture):
	""")
	
	GenerateTestFixture("warnings", "BooCompiler.Tests/CompilerWarningsTestFixture.generated.boo", "BooCompiler.CompilerWarnings", """
	namespace BooCompiler.Tests

	import NUnit.Framework
	import Boo.Lang.Compiler

	[TestFixture]
	class CompilerWarningsTestFixture(AbstractCompilerTestCase):
		protected override def SetUpCompilerPipeline() as CompilerPipeline:
			pipeline as CompilerPipeline = Boo.Lang.Compiler.Pipelines.Compile()
			pipeline.Add(Boo.Lang.Compiler.Steps.PrintWarnings())
			return pipeline
	""")
	
	GenerateTestFixture("macros", "BooCompiler.Tests/MacrosTestFixture.generated.boo", "BooCompiler.Macros", """
	namespace BooCompiler.Tests

	import NUnit.Framework

	[TestFixture]
	class MacrosTestFixture(AbstractCompilerTestCase):
	""")
	
	GenerateTestFixture("stdlib", "BooCompiler.Tests/StdlibTestFixture.generated.boo", "BooCompiler.Stdlib", """
	namespace BooCompiler.Tests

	import NUnit.Framework

	[TestFixture]
	class StdlibTestFixture(AbstractCompilerTestCase):
	""")
	
	GenerateTestFixture("async", "BooCompiler.Tests/AsyncTestFixture.generated.boo", "BooCompiler.Async", """
	// Test suite ported from Roslyn tests found at
	// https://github.com/dotnet/roslyn/blob/master/src/Compilers/CSharp/Test/Emit/CodeGen/CodeGenAsyncTests.cs
	namespace BooCompiler.Tests

	import NUnit.Framework

	[TestFixture]
	class AsyncTestFixture(AbstractCompilerTestCase):
	""")

	GenerateTestFixture("attributes", "BooCompiler.Tests/AttributesTestFixture.generated.boo", "BooCompiler.Attributes", """
	namespace BooCompiler.Tests

	import NUnit.Framework
	import Boo.Lang.Compiler
	import Boo.Lang.Compiler.Steps

	[TestFixture]
	class AttributesTestFixture(AbstractCompilerTestCase):
		override protected def SetUpCompilerPipeline() as CompilerPipeline:
			pipeline as CompilerPipeline = Boo.Lang.Compiler.Pipelines.ExpandMacros()
			pipeline.Add(PrintBoo())
			return pipeline
	""")
	
	GenerateTestFixture("parser/roundtrip", "Boo.Lang.Parser.Tests/ParserRoundtripTestFixture.generated.boo", "Boo.Lang.Parser", """
	namespace Boo.Lang.Parser.Tests

	import NUnit.Framework

	[TestFixture]
	class ParserRoundtripTestFixture(AbstractParserTestFixture):
		def RunCompilerTestCase(fname as string):
			RunParserTestCase(fname)
	""")
	
	PortParserTestCases()
	GenerateTestFixture("parser/wsa", "Boo.Lang.Parser.Tests/WSAParserRoundtripTestFixture.generated.boo", "Boo.Lang.Parser", """
	namespace Boo.Lang.Parser.Tests

	import NUnit.Framework

	[TestFixture]
	class WSAParserRoundtripTestFixture(AbstractWSAParserTestFixture):
		def RunCompilerTestCase(fname as string):
			RunParserTestCase(fname)
	""")
	
	GenerateTestFixture("semantics", "BooCompiler.Tests/SemanticsTestFixture.generated.boo", "BooCompiler.Semantics", """
	namespace BooCompiler.Tests

	import NUnit.Framework
	import Boo.Lang.Compiler
	import Boo.Lang.Compiler.Pipelines

	[TestFixture]
	class SemanticsTestFixture(AbstractCompilerTestCase):
		protected override def SetUpCompilerPipeline() as CompilerPipeline:
			return CompileToBoo()
	""")
	
	GenerateTestFixture("ducky", "BooCompiler.Tests/DuckyTestFixture.generated.boo", "BooCompiler.Ducky", """
	namespace BooCompiler.Tests

	import NUnit.Framework
	import Boo.Lang.Compiler
	import Boo.Lang.Compiler.Pipelines

	[TestFixture]
	class DuckyTestFixture(AbstractCompilerTestCase):
		protected override def CustomizeCompilerParameters():
			_parameters.Ducky = true
	""")
	
	GenerateTestFixture("net2/generics", "BooCompiler.Tests/GenericsTestFixture.generated.boo", "BooCompiler.Generics", """
	namespace BooCompiler.Tests

	import NUnit.Framework

	[TestFixture]
	class GenericsTestFixture(AbstractCompilerTestCase):
		override protected def RunCompilerTestCase(name as string):
			Assert.Ignore("Test requires .net 2.") if System.Environment.Version.Major < 2
			resolver as System.ResolveEventHandler = InstallAssemblyResolver(BaseTestCasesPath)
			try:
				super.RunCompilerTestCase(name)
			ensure:
				RemoveAssemblyResolver(resolver)

		override protected def CopyDependencies():
			CopyAssembliesFromTestCasePath()
	""")
	
	GenerateTestFixture("net2/errors", "BooCompiler.Tests/Net2ErrorsTestFixture.generated.boo", "BooCompiler.Net2Errors", """
	namespace BooCompiler.Tests

	import NUnit.Framework

	[TestFixture]
	class Net2ErrorsTestFixture(AbstractCompilerErrorsTestFixture):
		override protected def RunCompilerTestCase(name as string):
			Assert.Ignore("Test requires .net 2.") if System.Environment.Version.Major < 2
			super.RunCompilerTestCase(name)
	""")
	
	GenerateTestFixture("unsafe", "BooCompiler.Tests/UnsafeTestFixture.generated.boo", "BooCompiler.Unsafe", """
	namespace BooCompiler.Tests

	import NUnit.Framework

	[TestFixture]
	class UnsafeTestFixture(AbstractCompilerTestCase):
		protected override def CustomizeCompilerParameters():
			_parameters.Unsafe = true

		protected override VerifyGeneratedAssemblies as bool:
			get: return false
	""")
	
	GenerateTestFixture("unsafe/errors", "BooCompiler.Tests/UnsafeErrorsTestFixture.generated.boo", "BooCompiler.UnsafeErrors", """
	namespace BooCompiler.Tests

	import NUnit.Framework

	[TestFixture]
	class UnsafeErrorsTestFixture(AbstractCompilerErrorsTestFixture):
		protected override def CustomizeCompilerParameters():
			_parameters.Unsafe = true
	""")
	

def PortParserTestCases():
"""
Generates WSA parser test cases from
normal parser test cases.
"""
	for testcase in List of string(Directory.GetFiles("testcases/parser/roundtrip", "*.boo")).Sort():
		if not testcase.EndsWith(".boo"): continue

		fname = Path.GetFileName(testcase)
		wsaTestCase = Path.Combine("testcases/parser/wsa", fname)
		if not File.Exists(wsaTestCase):
			PortParserTestCase(testcase, wsaTestCase)
			print fname

def CopyDocstring(fname as string, writer as TextWriter):
	using file=File.OpenText(fname):
		tripleQuoteCount = 0
		for line in file:
			writer.WriteLine(line)
			if line.Trim() == '"""':
				++tripleQuoteCount
				break if tripleQuoteCount == 2

def PortParserTestCase(fromTestCase as string, toTestCase as string):
	using writer = StreamWriter(toTestCase):
		CopyDocstring(fromTestCase, writer)
		module=BooParser.ParseFile(fromTestCase).Modules[0]
		module.Accept(BooPrinterVisitor(writer, BooPrinterVisitor.PrintOptions.WSA))

def GetTestCaseName(fname as string):
	return join(
		(ch if char.IsLetterOrDigit(ch) else "_")
		for ch in Path.GetFileNameWithoutExtension(fname),"")

def MapPath(path):
#	return Path.Combine(Project.BaseDirectory, path)
	return Path.GetFullPath(path)

def WriteTestCases(writer as TextWriter, baseDir as string, boo as bool):
	count, ignored = 0, 0;
	testCasePath = MapPath("testcases/${baseDir}");
	System.IO.Directory.CreateDirectory(testCasePath);

	# Directory order is whatever the filesystem says, so it differs between
	# machines. Sort ordinally to keep a regenerated fixture stable everywhere.
	testCaseFiles = Directory.GetFiles(testCasePath)
	Array.Sort(testCaseFiles, StringComparer.Ordinal)

	for fname as string in testCaseFiles:
		continue unless fname.EndsWith(".boo")
		attribute = CategoryAttributeFor(fname)

		ignore = attribute.StartsWith("[Ignore")
		++count unless ignore
		++ignored if ignore

		name = GetTestCaseName(fname)
		testCase = NormalizePath(Path.GetFileName(fname))
		if boo:
			# A testcase is free to be named for a Boo keyword, so the method
			# it generates is always written escaped. The name it compiles to
			# is the same either way.
			writer.Write("""
	${attribute}[Test]
	def @${name}():
		RunCompilerTestCase("${testCase}")
""")
		else:
			writer.Write("""
		${attribute}[Test]
		public void ${name}()
		{
			RunCompilerTestCase(@"${testCase}");
		}
		""")
	print "{0,5} {1,7}  {2}" % (count, ignored, baseDir)
	
def CategoryAttributeFor(testFile as string):
"""
If the first line of the test case file starts with // category CategoryName 
then return a suitable [CategoryName()] attribute.
"""
	match FirstLineOf(testFile):
		case /\s*#\s*ignore\s+(?<reason>.*)/:
			return "[Ignore(\"${reason[0].Value.Trim()}\")]"
		case /\s*#\s*category\s+(?<name>.*)/:
			return "[Category(\"${name[0].Value.Trim()}\")]"
		case /\s*#\s*platform\s+(?<name>.*)/:
			return "[Platform(\"${name[0].Value.Trim()}\")]"
		otherwise:
			return ""
			
def FirstLineOf(fname as string):
	using reader=File.OpenText(fname):
		return reader.ReadLine()

def GenerateTestFixture(srcDir as string, targetFile as string, fixtureAssembly as string, header as string):
	boo = targetFile.EndsWith(".boo")
	using writer=StreamWriter(MapPath(targetFile)):
		# Boo has no closing braces to indent against, so the header stands on
		# its own rather than carrying the blank lines ReIndent leaves around it.
		if boo:
			writer.Write(ReIndent(header).Trim() + "\n")
		else:
			writer.Write(ReIndent(header))
		WriteTestCases(writer, srcDir, boo)
		if boo:
			writer.Write("""
	override protected def GetRelativeTestCasesPath() as string:
		return "${NormalizePath(srcDir)}"
""")
		else:
			writer.Write("""

		override protected string GetRelativeTestCasesPath()
		{
			return "${NormalizePath(srcDir)}";
		}
	}
}
""")

def NormalizePath(path as string):
	return path.Replace('\\', '/')

def GenerateIntegrationTestFixtures():
	for dir in Directory.GetDirectories("testcases/integration"):
		if /\.svn/.IsMatch(dir): continue
		GenerateIntegrationTestFixture("integration/${Path.GetFileName(dir)}")

def PascalCase(name as string):
	return name[:1].ToUpper() + name[1:]

def IntegrationTestFixtureName(dir as string):
	baseName = join(PascalCase(part) for part in /-/.Split(Path.GetFileName(dir)), '')
	return "${baseName}IntegrationTestFixture"

def GenerateIntegrationTestFixture(dir as string):
	fixtureName = IntegrationTestFixtureName(dir)
	header = """
	namespace BooCompiler.Tests

	import NUnit.Framework

	[TestFixture]
	class ${fixtureName}(AbstractCompilerTestCase):
	"""
	GenerateTestFixture(dir, "BooCompiler.Tests/${fixtureName}.generated.boo", "BooCompiler.$(fixtureName.Replace('TestFixture', ''))", header)

def ReIndent(code as string):	
	lines = code.Replace("\r\n", "\n").Split(char('\n'))
	nonEmptyLines = line for line in lines if len(line.Trim())

	indentation = /(\s*)/.Match(nonEmptyLines.First()).Groups[0].Value
	return code if len(indentation) == 0

	buffer = System.Text.StringBuilder()
	for line in lines:
		if line.StartsWith(indentation):
			buffer.AppendLine(line[len(indentation):])
		else:
			buffer.AppendLine(line)
	return buffer.ToString()
	
