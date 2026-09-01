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
//     * Neither the name of the Rodrigo B. de Oliveira nor the names of its
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

namespace WSABoo.Parser.Tests

import NUnit.Framework
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO
import Boo.Lang.Parser

[TestFixture]
class WSABooParserTestFixture:
	[Test]
	def SanityCheck():
		code = `
			class Foo(Bar):
				def foo():
					if foo:
					print 'foo'
					end
					if bar:
					    print 'bar'
					elif foo:
					    print 'foo'
					else:
						print 'uops...'
					end
				print 'foo again'
				end
				
				item[key]:
				get:
					return key
				end
				end
				
				def empty():
				end
			end
			`

		module = parse(code)

		expected = `
class Foo(Bar):

	def foo():
		if foo:
			print 'foo'
		if bar:
			print 'bar'
		elif foo:
			print 'foo'
		else:
			print 'uops...'
		print 'foo again'

	item[key]:
		get:
			return key

	def empty():
		pass
	`
		Assert.AreEqual(normalize(expected), normalize(module.ToCodeString()))

	[Test]
	def EmptyModule():
		Assert.AreEqual("", normalize(parse("").ToCodeString()))

	[Test]
	def SanityCheckUsingDoubleQuotes():
		code = `
			def SayHello(name as string):
				return "Hello, $name"
			end
			`

		module = parse(code)

		expected = `
def SayHello(name as string):
	return "Hello, $name"
	`
		Assert.AreEqual(normalize(expected), normalize(module.ToCodeString()))

	[Test]
	def NoLineBreakBeforeEOF():
		code = "print \"hello\""

		module = parse(code)

		expected = "print 'hello'"

		Assert.AreEqual(normalize(expected), normalize(module.ToCodeString()))

	[Test]
	def InputNameIsPreserved():
		inputName = "File.boo"
		module = parse(StringInput(inputName, "class Foo:\nend"))
		AssertInputName(inputName, module)
		for member as TypeMember in module.Members:
			AssertInputName(inputName, member)

	[Test]
	def NonBlockColons():
		inputName = "File.boo"
		module = parse(StringInput(inputName, "class Foo:\nlst = (of int: 1)\ndct = {'foo':\n\t'bar'}\nend"))
		AssertInputName(inputName, module)
		for member as TypeMember in module.Members:
			AssertInputName(inputName, member)

	[Test]
	def LabelColons():
		inputName = "File.boo"
		expected = ":label\nprint 'foo'\ngoto label"
		module = parse(StringInput(inputName, expected))
		AssertInputName(inputName, module)
		Assert.AreEqual(normalize(expected), normalize(module.ToCodeString()))

	[Test]
	def LogicalOr():
		code = "a = (false or true)"
		expected = code
		module = parse(StringInput("test", code))
		Assert.AreEqual(normalize(expected), normalize(module.ToCodeString()))

	[Test]
	def ForOr():
		code = "for i in items:\nor:\nend"
		expected = "for i in items:\n\tpass\nor:\n\tpass"
		module = parse(StringInput("test", code))
		Assert.AreEqual(normalize(expected), normalize(module.ToCodeString()))

	[Test]
	def Property():
		code =     "class Foo:\nprop as int:\nget: return 10\nend\nend\nend"
		expected = "class Foo:\n\n\tprop as int:\n\t\tget:\n\t\t\treturn 10"
		module = parse(StringInput("test", code))
		Assert.AreEqual(normalize(expected), normalize(module.ToCodeString()))

	[Test]
	def WithComment():
		code =     "def foo():# comment\nend"
		expected = "def foo():\n\tpass"
		module = parse(StringInput("test", code))
		Assert.AreEqual(normalize(expected), normalize(module.ToCodeString()))

	[Test]
	def DocStrings():
		code = "def foo():\n\"\"\" docu \"\"\"\nend"
		module = parse(StringInput("test", code))
		for member as TypeMember in module.Members:
			Assert.AreEqual(member.Documentation, " docu ")

	private def AssertInputName(inputName as string, module as Node):
		Assert.AreEqual(inputName, module.LexicalInfo.FileName)

	def normalize(s as string) as string:
		return s.Trim().Replace("\r\n", "\n")

	def parse(code as string) as Module:
		input = StringInput("code", code)
		return parse(input)

	def parse(input as StringInput) as Module:
		pipeline = CompilerPipeline()
		pipeline.Add(WSABooParsingStep())

		compiler = BooCompiler()
		compiler.Parameters.Pipeline = pipeline
		compiler.Parameters.Input.Add(input)
		result = compiler.Run()
		Assert.AreEqual(0, result.Errors.Count, result.Errors.ToString())
		Assert.AreEqual(1, result.CompileUnit.Modules.Count)
		return result.CompileUnit.Modules[0]
