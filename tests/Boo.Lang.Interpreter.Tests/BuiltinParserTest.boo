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

namespace Boo.Lang.Interpreter.Tests

import System
import NUnit.Framework
import Boo.Lang.Interpreter.ShellCmd

[TestFixture]
class BuiltinParserTest:
"""Tests the parser for builtin commands."""
	public def constructor():
		pass
	
	[TestCase("/d argument1 argument2", "d\nargument1\nargument2")]
	[TestCase("/d (argument 1) \"argument 2\"", "d\nargument 1\nargument 2")]
	[TestCase("describe [argument  1] {arg \"ument 2}", "describe\nargument  1\narg \"ument 2")]
	[TestCase("describe   'arg'ument('(  1'", "describe\narg\nument('(\n1'")]
	[TestCase("describe 'arg''ument(''(  1'", "describe\narg\nument(\n(  1")]
	[TestCase("describe [{[]{{\"] arg2", "describe\n{[]{{\"\narg2")]
	public def Test(line as string, reference as string):
		references = reference.Split("\n"[0])
		p=Parser(line)
		assert references[0].Equals(p.Cmd), references[0]+" != "+p.Cmd
		for i in range(1, references.Length):
			assert references[i].Equals(p.Args[i-1]), references[i]+" != "+p.Args[i-1]
	
	public def TestOneArgument():
		p = Parser("d [| function argument |] ")
		// OK, we noticed, that d only accepts one argument
		p.SetOnlyOneArgument()
		assert 1.Equals(p.Args.Length), "Expected 1 Arg got "+p.Args.Length
		assert "[| function argument |]".Equals(p.Args[0]), "[| function argument |] != " + p.Args[0].ToString()