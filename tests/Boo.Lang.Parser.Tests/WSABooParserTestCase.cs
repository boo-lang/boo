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
{

	using NUnit.Framework;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.IO;
	using Boo.Lang.Parser;

	[TestFixture]
	public class WSABooParserTestFixture
	{	
		[Test]
		public void SanityCheck()
		{		
			string code = @"
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
			";
			
			Module module = parse(code);
			
			string expected = @"
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
	";
			Assert.AreEqual(normalize(expected), normalize(module.ToCodeString()));
		}
			
		[Test]
		public void SanityCheckUsingDoubleQuotes()
		{
			string code = @"
			def SayHello(name as string):
				return ""Hello, ${name}""
			end
			";
			
			Module module = parse(code);
			
			string expected = @"
def SayHello(name as string):
	return ""Hello, ${name}""
	";
			Assert.AreEqual(normalize(expected), normalize(module.ToCodeString()));
		}
		
		[Test]
		public void NoLineBreakBeforeEOF()
		{
			string code = "print \"hello\"";
			
			Module module = parse(code);
			
			string expected = "print 'hello'";
			
			Assert.AreEqual(normalize(expected), normalize(module.ToCodeString()));
		}
		
		string normalize(string s)
		{
			return s.Trim().Replace("\r\n", "\n");
		}
			
		Module parse(string code)
		{
			CompilerPipeline pipeline = new CompilerPipeline();
			pipeline.Add(new WSABooParsingStep());
			
			BooCompiler compiler = new BooCompiler();
			compiler.Parameters.Pipeline = pipeline;
			compiler.Parameters.Input.Add(new StringInput("code", code));
			CompilerContext result = compiler.Run();
			Assert.AreEqual(0, result.Errors.Count, result.Errors.ToString());
			Assert.AreEqual(1, result.CompileUnit.Modules.Count);
			return result.CompileUnit.Modules[0];
		}
	}
}
			
