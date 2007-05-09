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

class BooCommandLine(AbstractCommandLine):
	
	[Option("boo.BooCommandLine.target", ShortForm: "t", LongForm: "target")]
	public Target as string
	
	[Option("boo.BooCommandLine.output", ShortForm: "o", LongForm: "output")]
	public Output as string
	
	[getter(Arguments)]
	_args = []
	
	[getter(References)]
	_references = []
	
	[Option("boo.BooCommandLine.reference", ShortForm: "r", MaxOccurs: int.MaxValue)]
	def OnReference([required] reference as string):
		_references.Add(reference)
	
	[Argument("boo.BooCommandLine.argument")]
	def OnArgument([required] value as string):
		_args.Add(value)
		
	[Option("boo.BooCommandLine.resource", LongForm: "res", MaxOccurs: int.MaxValue)]
	public Resources = []
	
	[Option("boo.BooCommandLine.debug")] # long form deduced from the field name
	public Debug = false
		
	def constructor(argv as (string)):
		Parse(argv)
		
class AnotherCommandLine(AbstractCommandLine):
	
	[Argument("boo.AnotherCommandLine.argument")]
	public Arguments = []
	
	def constructor(argv as (string)):
		Parse(argv)
		
class JiraCommandLine(AbstractCommandLine):
	
	[Option("jira.login", ShortForm: "l", LongForm: "jira-login", MinOccurs: 1, MaxOccurs: 1)]
	public JiraLogin as string
	
	def constructor(argv as (string)):
		Parse(argv)
		
[TestFixture]
class AbstractCommandLineTestFixture:

	[Test]
	def TestParse():
		argv = (
			"-t:exe",
			"foo.boo",
			"-output:bin/foo.exe",
			"bar.boo",
			"-res:res1",
			"-res:res2,id2",
			"-debug",
			"-r:Foo.dll",
			"-r:Bar.dll",
		)
		
		cmdLine = BooCommandLine(argv)
		
		Assert.AreEqual("exe", cmdLine.Target)
		Assert.AreEqual("bin/foo.exe", cmdLine.Output)
		Assert.AreEqual(["foo.boo", "bar.boo"], cmdLine.Arguments)
		Assert.AreEqual(["res1", "res2,id2"], cmdLine.Resources)
		Assert.AreEqual(["Foo.dll", "Bar.dll"], cmdLine.References)
		assert cmdLine.Debug
		
	[Test]
	def TestBooleanParse():
		
		argv = ("-debug-",)
		assert not BooCommandLine(argv).Debug
		
		argv = ("-debug+",)
		assert BooCommandLine(argv).Debug
		
	[Test]
	def TestArgumentToListField():
		
		argv = ("arg1", "arg2")
		cmdLine = AnotherCommandLine(argv)
		
		Assert.AreEqual(["arg1", "arg2"], cmdLine.Arguments)
		
	[Test]
	def TestOptionWithDashes():
		argv = ("-jira-login:foo",)
		cmdLine = JiraCommandLine(argv)
		Assert.AreEqual("foo", cmdLine.JiraLogin)
