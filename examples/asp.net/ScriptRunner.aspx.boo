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

namespace Boo.Examples.Web

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps
import System
import System.IO
import System.Web
import System.Web.UI
import System.Web.UI.WebControls
import System.Web.UI.HtmlControls

class WebMacro:
	Console = StringWriter()	
	Context = HttpContext.Current
	Request = Context.Request
	Response = Context.Response
	
	virtual def print(text):
		Console.WriteLine(text)
		
	override def ToString():
		return Console.ToString()
		
	abstract def Run():
		pass
		
class CreateMacroStep(AbstractCompilerStep):
	
	override def Run():
		module = CompileUnit.Modules[0]
		
		method = Method(Name: "Run",
						Modifiers: TypeMemberModifiers.Override,
						Body: module.Globals)
						
		module.Globals = Block()
		
		macro = ClassDefinition(Name: "__Macro__")
		macro.BaseTypes.Add(SimpleTypeReference("Boo.Examples.Web.WebMacro"))
		macro.Members.Add(method)
		
		for member in module.Members:
			macro.Members.Add(member)
		
		module.Members.Clear()
		module.Members.Add(macro)
		

class ScriptRunnerPage(Page):

	_code as TextBox
	_console as HtmlGenericControl
	
	def _run_Click(sender, args as EventArgs):
		result = CompileMacro(_code.Text)
		if len(result.Errors):
			WriteConsole(join(result.Errors, "\n"))
		else:
			type = result.GeneratedAssembly.GetType("__Macro__")
			macro = cast(WebMacro, type())
			macro.Run()
			WriteConsole(macro.ToString())
		
	def CompileMacro(code):
		compiler = BooCompiler()
		compiler.Parameters.Pipeline = CompileToMemory()
		compiler.Parameters.Input.Add(StringInput("<code>", code))
		compiler.Parameters.OutputType = CompilerOutputType.Library
		compiler.Parameters.References.Add(typeof(WebMacro).Assembly)
		pipeline = compiler.Parameters.Pipeline
		pipeline.Insert(1, CreateMacroStep())
		return compiler.Run()		
			
	def WriteConsole(text as string):
		_console.InnerHtml = Server.HtmlEncode(text).Replace("\n", "<br />")
		
		
