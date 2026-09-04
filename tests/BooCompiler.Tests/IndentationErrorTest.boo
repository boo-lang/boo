#region license
// Copyright (c) 2026 the Boo contributors
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

namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class IndentationErrorTest:
"""
What the parser says when the layout is wrong.

Naming the token says nothing in a language where the layout is the syntax.
"""

	private def FirstError(code as string) as string:
		compiler = Boo.Lang.Compiler.BooCompiler()
		compiler.Parameters.Input.Add(Boo.Lang.Compiler.IO.StringInput("testcase", code))
		compiler.Parameters.Pipeline = Boo.Lang.Compiler.Pipelines.Parse()
		errors = compiler.Run().Errors
		Assert.IsTrue(errors.Count >= 1, "nothing was reported")
		return errors[0].Message

	[Test]
	def SaysWhatIsWrongWhenABodyIsIndentedTooFar():
		# The docstring is a level further in than the class body it opens.
		message = FirstError("class Greeter:\n\t\t\"\"\"over indented\"\"\"\n")
		Assert.AreEqual("Indentation does not line up with any block that is open.", message)

	[Test]
	def StillSaysABlockMustBeIndented():
		Assert.AreEqual("Block must be indented.", FirstError("foo:\nclass"))

	[Test]
	def PointsAtTheStartOfTheLineNotPastItsEnd():
		compiler = Boo.Lang.Compiler.BooCompiler()
		compiler.Parameters.Input.Add(Boo.Lang.Compiler.IO.StringInput("testcase", "class Greeter:\n\t\t\"\"\"over indented\"\"\"\n"))
		compiler.Parameters.Pipeline = Boo.Lang.Compiler.Pipelines.Parse()
		error = compiler.Run().Errors[0]
		Assert.AreEqual(2, error.LexicalInfo.Line)
		Assert.AreEqual(1, error.LexicalInfo.Column)
