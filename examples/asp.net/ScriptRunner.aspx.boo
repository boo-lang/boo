#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
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
		
		
