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
	
	GenerateTestFixture("regression", "BooCompiler.Tests", "RegressionTestFixture", "AbstractCompilerTestCase")
	
	GenerateTestFixture("errors", "BooCompiler.Tests", "CompilerErrorsTestFixture", "AbstractCompilerErrorsTestFixture")
	
	GenerateTestFixture("warnings", "BooCompiler.Tests", "CompilerWarningsTestFixture", "AbstractCompilerTestCase")
	
	GenerateTestFixture("macros", "BooCompiler.Tests", "MacrosTestFixture", "AbstractCompilerTestCase")
	
	GenerateTestFixture("stdlib", "BooCompiler.Tests", "StdlibTestFixture", "AbstractCompilerTestCase")
	
	GenerateTestFixture("async", "BooCompiler.Tests", "AsyncTestFixture", "AbstractCompilerTestCase")

	GenerateTestFixture("attributes", "BooCompiler.Tests", "AttributesTestFixture", "AbstractCompilerTestCase")
	
	GenerateTestFixture("parser/roundtrip", "Boo.Lang.Parser.Tests", "ParserRoundtripTestFixture", "AbstractParserTestFixture")
	
	PortParserTestCases()
	GenerateTestFixture("parser/wsa", "Boo.Lang.Parser.Tests", "WSAParserRoundtripTestFixture", "AbstractWSAParserTestFixture")
	
	GenerateTestFixture("semantics", "BooCompiler.Tests", "SemanticsTestFixture", "AbstractCompilerTestCase")
	
	GenerateTestFixture("ducky", "BooCompiler.Tests", "DuckyTestFixture", "AbstractCompilerTestCase")
	
	GenerateTestFixture("net2/generics", "BooCompiler.Tests", "GenericsTestFixture", "AbstractCompilerTestCase")
	
	GenerateTestFixture("net2/errors", "BooCompiler.Tests", "Net2ErrorsTestFixture", "AbstractCompilerErrorsTestFixture")
	
	GenerateTestFixture("unsafe", "BooCompiler.Tests", "UnsafeTestFixture", "AbstractCompilerTestCase")
	
	GenerateTestFixture("unsafe/errors", "BooCompiler.Tests", "UnsafeErrorsTestFixture", "AbstractCompilerErrorsTestFixture")
	
	GenerateTestFixture("byreflike", "BooCompiler.Tests", "RefStructsTestFixture", "AbstractCompilerTestCase")
	
	GenerateTestFixture("byreflike/errors", "BooCompiler.Tests", "RefStructsErrorsTestFixture", "AbstractCompilerErrorsTestFixture")
	

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

def WriteTestCases(writer as TextWriter, baseDir as string):
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
		# A testcase is free to be named for a Boo keyword, so the method it
		# generates is always written escaped. The name it compiles to is the
		# same either way.
		writer.Write("""
	${attribute}[Test]
	def @${name}():
		RunCompilerTestCase("${testCase}")
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

def GenerateTestFixture(srcDir as string, project as string, fixtureName as string, baseClass as string):
	"""
	Writes one test per testcase file.

	A fixture with nothing to say beyond its testcases is written whole. One
	that overrides something keeps a hand written half declaring the class,
	and this becomes the other half of the partial, so what is said by hand
	survives a regeneration.
	"""
	handWritten = File.Exists(MapPath("${project}/${fixtureName}.boo"))

	declaration = "partial class ${fixtureName}:"
	unless handWritten:
		declaration = "[TestFixture]\nclass ${fixtureName}(${baseClass}):"

	System.IO.Directory.CreateDirectory(MapPath("${project}/Generated"))
	using writer=StreamWriter(MapPath("${project}/Generated/${fixtureName}.generated.boo")):
		writer.Write("""namespace ${project}

import NUnit.Framework

${declaration}
""")
		WriteTestCases(writer, srcDir)
		writer.Write("""
	override protected def GetRelativeTestCasesPath() as string:
		return "${NormalizePath(srcDir)}"
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
	GenerateTestFixture(dir, "BooCompiler.Tests", IntegrationTestFixtureName(dir), "AbstractCompilerTestCase")

	
