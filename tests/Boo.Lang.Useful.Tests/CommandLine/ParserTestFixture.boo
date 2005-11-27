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

namespace Boo.Lang.Useful.CommandLine.Tests

import System
import Useful.CommandLine
import NUnit.Framework

[TestFixture]
class ParserTestFixture:

	[Test]
	def TestSimpleOptions():
		args = ("-b", "-r:foo.dll,bar.dll", "--namespace", "foo.boo", "bar.boo")

		bWasFound = false
		rWasFound = false
		namespaceWasFound = false

		bHandler = def (value as string):
			bWasFound = true
			assert value is null
			
		rHandler = def (value as string):
			rWasFound = true
			assert value == "foo.dll,bar.dll"
			
		namespaceHandler = def (value as string):
			namespaceWasFound = true
			assert value is null

		parser = Parser()
		parser.AddOption(OptionAttribute(LongForm: "b"), bHandler)
		parser.AddOption(OptionAttribute(ShortForm: "r", LongForm: "reference"), rHandler)
		parser.AddOption(OptionAttribute(LongForm: "namespace"), namespaceHandler)
		
		arguments = []
		parser.ArgumentFound += def (value as string):
			arguments.Add(value)
			
		parser.Parse(args)
			
		assert bWasFound
		assert rWasFound
		assert namespaceWasFound
		assert arguments == ["foo.boo", "bar.boo"]
		
	[Test]
	[ExpectedException(CommandLineException)]
	def TestMinOccurs():
		parser = Parser()
		parser.AddOption(OptionAttribute(ShortForm: "b", LongForm: "boo", MinOccurs: 1), DoNothing)
		parser.Parse(("f", ))
		
	[Test]
	[ExpectedException(CommandLineException)]
	def TestMaxOccurs():
		parser = Parser()
		parser.AddOption(OptionAttribute(ShortForm: "b", LongForm: "boo", MaxOccurs: 1), DoNothing)
		parser.Parse(("-b", "-b"))
		
	def DoNothing(value as string):
		pass
