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
import System.Linq

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
	def ImportsComeAfterNamespaceDeclaration():
		ns = CodeNamespace("A.B.C")
		ns.Imports.Add(CodeNamespaceImport("D.E.F"))
		unit = CodeCompileUnit()
		unit.Namespaces.Add(ns)
		AssertCompileUnitIgnoringComments "namespace A.B.C\nimport D.E.F", unit
		
	[Test]
	def AttributesWithoutArgumentsAreGeneratedWithoutParens():
		type = CodeTypeDeclaration("Foo")
		type.CustomAttributes.Add(CodeAttributeDeclaration("Test"))
		AssertTypeDeclaration "[Test]\nclass Foo:\n    pass", type
		
	[Test]
	def AttributesWithArgumentsAreGeneratedWithParens():
		attribute = CodeAttributeDeclaration("Test")
		attribute.Arguments.Add(CodeAttributeArgument(CodePrimitiveExpression(42)))
		type = CodeTypeDeclaration("Foo")
		type.CustomAttributes.Add(attribute)
		AssertTypeDeclaration "[Test(42)]\nclass Foo:\n    pass", type
		
	[Test]
	def Enums():
		type = CodeTypeDeclaration("Foo", IsEnum: true)
		type.Members.Add(CodeMemberField(type.Name, "Bar"))
		type.Members.Add(CodeMemberField(type.Name, "Baz"))
		AssertTypeDeclaration "enum Foo:\n    \n    Bar\n    Baz", type

	[Test]
	def TestNestedTypeReference():
		AssertExpression "System.Environment.SpecialFolder", CodeTypeReferenceExpression(System.Environment.SpecialFolder)
		
	[Test]
	def TestNullableType():
		AssertType "System.Nullable[of int]", System.Nullable of int
		
	def AssertType(expected as string, type as System.Type):
		stmt = CodeVariableDeclarationStatement(Name: "foo", Type: CodeTypeReference(type))
		AssertStatement "foo as ${expected}", stmt
		
	[Test]
	def TestArrayOfGenericType():
		AssertType(
			"(System.Collections.Generic.Dictionary[of string, int])",
			typeof((System.Collections.Generic.Dictionary[of string, int])))

	[Test]
	def TestArrayType():
		stmt = CodeVariableDeclarationStatement(
					Name: "anArray",
					Type: CodeTypeReference(typeof((int))))
					
		AssertStatement "anArray as (int)", stmt
		
	[Test]
	def TestArrayCreateSize():
		e = CodeArrayCreateExpression(CodeTypeReference(int), 10)
		
		AssertExpression "array(int, 10)", e
		
	[Test]
	def TestArrayCreateSizeExpression():
		e = CodeArrayCreateExpression(CodeTypeReference(int), 
			CodeVariableReferenceExpression("sz"))
		
		AssertExpression "array(int, sz)", e
		
	[Test]
	def TestArrayCreateSingle():
		e = CodeArrayCreateExpression(CodeTypeReference(int), *(CodePrimitiveExpression(2),))
		AssertExpression "(of int: 2)", e
		
	[Test]
	def TestArrayCreateMultiple():
		e = CodeArrayCreateExpression(CodeTypeReference(int), CodePrimitiveExpression(2),
			CodePrimitiveExpression(3), CodePrimitiveExpression(4))
		
		AssertExpression "(of int: 2, 3, 4)", e

	[Test]
	def TestPartial():
		expected = "partial class PartialType:\n    pass"
		AssertTypeDeclaration expected, CodeTypeDeclaration("PartialType", IsPartial: true)
	
	[Test]
	def TestCharType():
		e = CodePrimitiveExpression(char('a'))
		AssertExpression "char('a')", e
	
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
		AssertEqualsIgnoringNewLines expected, result

	[Test]
	def CodeSnippetTypeMemberTest():
		compileUnit = CodeCompileUnit()
		nspace = CodeNamespace("TestNamespace")
		compileUnit.Namespaces.Add(nspace)
		cls = CodeTypeDeclaration("Test", IsClass: true)
		mem = CodeSnippetTypeMember()
		mem.Text = """
def TestFunc():
	pass
"""
		cls.Members.Add(mem)
		nspace.Types.Add(cls)

		codegen = BooCodeGenerator()
		expected = """namespace TestNamespace


class Test:
    

    def TestFunc():
        pass
"""

		AssertCompileUnitIgnoringComments expected, compileUnit
		
	def AssertEqualsIgnoringNewLines(expected as string, actual as string):
		Assert.AreEqual(NormalizeNewLines(expected), NormalizeNewLines(actual))
	
	def NormalizeNewLines(s as string):
		return s.Trim().Replace("\r\n", "\n")
		
	def AssertCompileUnitIgnoringComments(expected as string, unit as CodeCompileUnit):
		code = CodeFromCompileUnit(unit)
		AssertEqualsIgnoringNewLines expected, RemoveCommentedLines(code)
		
	def RemoveCommentedLines(code as string):
		return string.Join("\n", code.Split(char('\n')).Where({ line | not line.StartsWith("#") }).ToArray())
		
	def CodeFromCompileUnit(unit as CodeCompileUnit):
		buffer = StringWriter()
		_generator.GenerateCodeFromCompileUnit(unit, buffer, CodeGeneratorOptions())
		return buffer.ToString()

	def AssertTypeDeclaration(expected as string, type as CodeTypeDeclaration):
		AssertEqualsIgnoringNewLines expected, CodeFromTypeDeclaration(type)
		
	def CodeFromTypeDeclaration(type as CodeTypeDeclaration):
		buffer = StringWriter()
		_generator.GenerateCodeFromType(type, buffer, CodeGeneratorOptions())
		return buffer.ToString()
		
	def AssertStatement(expected as string, stmt as CodeStatement):
		AssertEqualsIgnoringNewLines expected, CodeFromStatement(stmt)
		
	def CodeFromStatement(stmt as CodeStatement):
		buffer = StringWriter()
		_generator.GenerateCodeFromStatement(stmt, buffer, CodeGeneratorOptions())
		return buffer.ToString()
		
	def AssertExpression(expected as string, e as CodeExpression):
		AssertEqualsIgnoringNewLines expected, CodeFromExpression(e)
		
	def CodeFromExpression(e as CodeExpression):
		buffer = StringWriter()
		_generator.GenerateCodeFromExpression(e, buffer, CodeGeneratorOptions())		
		return buffer.ToString()
