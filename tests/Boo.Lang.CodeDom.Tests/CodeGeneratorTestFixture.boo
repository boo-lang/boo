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

namespace Boo.CodeDom.Tests

import System
import System.CodeDom
import System.CodeDom.Compiler
import System.IO
import NUnit.Framework
import Boo.Lang.CodeDom

[TestFixture]
class CodeGeneratorTestFixture:
	
	_generator as ICodeGenerator
	
	[SetUp]
	def SetUp():		
		_generator = BooCodeProvider().CreateGenerator()
		Assert.IsNotNull(_generator)

	[Test]
	def TestNestedTypeReference():
		buffer = StringWriter()
		_generator.GenerateCodeFromExpression(CodeTypeReferenceExpression(System.Environment.SpecialFolder), buffer, CodeGeneratorOptions())
		Assert.AreEqual("System.Environment.SpecialFolder", buffer.ToString().Trim())

	
	[Test]
	def TestArrayType():
		stmt = CodeVariableDeclarationStatement()
		stmt.Name = "anArray"
		stmt.Type = CodeTypeReference(typeof((int)))
		
		expected = "anArray as (int)"
		
		buffer = StringWriter()
		_generator.GenerateCodeFromStatement(stmt, buffer, CodeGeneratorOptions())
		Assert.AreEqual(expected, buffer.ToString().Trim())
		
	[Test]
	def TestArrayCreateSize():
		e = CodeArrayCreateExpression(CodeTypeReference(int), 10)
		
		expected = "array(int, 10)"
		
		buffer = StringWriter()
		_generator.GenerateCodeFromExpression(e, buffer, CodeGeneratorOptions())
		Assert.AreEqual(expected, buffer.ToString().Trim())
		
	[Test]
	def TestArrayCreateSizeExpression():
		e = CodeArrayCreateExpression(CodeTypeReference(int), 
			CodeVariableReferenceExpression("sz"))
		
		expected = "array(int, sz)"
		
		buffer = StringWriter()
		_generator.GenerateCodeFromExpression(e, buffer, CodeGeneratorOptions())
		Assert.AreEqual(expected, buffer.ToString().Trim())
		
	[Test]
	def TestArrayCreateSingle():
		e = CodeArrayCreateExpression(CodeTypeReference(int), *(CodePrimitiveExpression(2),))
		
		expected = "(of int: 2)"
		
		buffer = StringWriter()
		_generator.GenerateCodeFromExpression(e, buffer, CodeGeneratorOptions())
		Assert.AreEqual(expected, buffer.ToString().Trim())
		
	[Test]
	def TestArrayCreateMultiple():
		e = CodeArrayCreateExpression(CodeTypeReference(int), CodePrimitiveExpression(2),
			CodePrimitiveExpression(3), CodePrimitiveExpression(4))
		
		expected = "(of int: 2, 3, 4)"
		
		buffer = StringWriter()
		_generator.GenerateCodeFromExpression(e, buffer, CodeGeneratorOptions())
		Assert.AreEqual(expected, buffer.ToString().Trim())
	
	[Test]
	def TestCharType():
		e = CodePrimitiveExpression(char('a'))
		expected = "char('a')"
		
		buffer = StringWriter()
		_generator.GenerateCodeFromExpression(e, buffer, CodeGeneratorOptions())
		Assert.AreEqual(expected, buffer.ToString().Trim())
	
	[Test]
	def TestFixIndent1():
		//1. code is indented one tab, but needs to be indented 8 spaces.
		//2. also, there are comments before the code that should be ignored
		//3. also, stuff inside triple quoted strings should not be altered
		//3. extra leading whitespace converted to spaces as well,
		//   so tabs and spaces aren't mixed on the same line
		code = """
	//asdf
	   #asgdawg
/* 
	 Inline boo code 
	 	/*is 
		supported:
		*/
	 */
	def Calendar1Selected():
		Label1.Text = ("""
		code+='"""'
		code+="""Boo for .NET 
	says 
		you picked """
		code+='"""'
		code+=""" + Calendar1.SelectedDate.ToString('D'))

	def Button1Click():
		Calendar1.VisibleDate = System.Convert.ToDateTime(Edit1.Text)
		Label1.Text = 'Boo for .NET says you set ' + Calendar1.VisibleDate.ToString('D')
	
"""

		expected = """
        	//asdf
        	   #asgdawg
        /* 
        	 Inline boo code 
        	 	/*is 
        		supported:
        		*/
        	 */
        def Calendar1Selected():
            Label1.Text = ("""
		expected+='"""'
		expected+="""Boo for .NET 
	says 
		you picked """
		expected+='"""'
		expected+=""" + Calendar1.SelectedDate.ToString('D'))
        
        def Button1Click():
            Calendar1.VisibleDate = System.Convert.ToDateTime(Edit1.Text)
            Label1.Text = 'Boo for .NET says you set ' + Calendar1.VisibleDate.ToString('D')
        
        
"""
		
		result = Boo.Lang.CodeDom.BooCodeGenerator.FixIndent(code, "    ", 2, false)
		result = result.Replace("\r\n","\n").Trim()
		expected = expected.Replace("\r\n","\n").Trim()
		Assert.AreEqual(expected, result)

