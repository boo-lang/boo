#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.NAnt

import System.Diagnostics
import System.IO
import NAnt.Core
import NAnt.Core.Attributes
import NAnt.Core.Types
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps

class AbstractScript:
	
	[property(Project)]
	_project as Project
	
	[property(Task)]
	_task as BooTask
	
	def print(msg):
		_task.LogInfo(msg)
	
	abstract def Run():
		pass

class PrepareScriptStep(AbstractCompilerStep):
	
	override def Run():
		module = CompileUnit.Modules[0]
		
		method = Method(Name: "Run",
						Modifiers: TypeMemberModifiers.Override,
						Body: module.Globals)
						
		module.Globals = Block()
		
		script = ClassDefinition(Name: "__Script__")
		script.BaseTypes.Add(SimpleTypeReference("Boo.NAnt.AbstractScript"))
		script.Members.Add(method)
		
		for member in module.Members:
			script.Members.Add(member)
		
		module.Members.Clear()
		module.Members.Add(script)		
	

[TaskName("boo")]
class BooTask(AbstractBooTask):
	override protected def ExecuteTask():
		code = self.XmlNode.InnerText
		
		compiler = BooCompiler()
		parameters = compiler.Parameters
		parameters.OutputType = CompilerOutputType.Library
		parameters.Pipeline = CompileToMemory()
		parameters.Pipeline.Insert(1, PrepareScriptStep())
		parameters.Input.Add(StringInput("boo", reindent(code)))
		parameters.References.Add(typeof(BooTask).Assembly)
		parameters.References.Add(typeof(NAnt.Core.Project).Assembly)
		
		result = compiler.Run()
		CheckCompilationResult(result)
		
		try:
			scriptType = result.GeneratedAssembly.GetType("__Script__", true)
			script as AbstractScript = scriptType()
			script.Project = Project
			script.Task = self
			script.Run()
		except x:
			raise BuildException(x.Message, Location, x)
			
	def reindent(code as string):
		lines = /\n/.Split(code.Replace("\r\n", "\n"))
		lines = [line for line in lines if len(line.Trim())].ToArray(string)
	
		first = lines[0]
		indent = /(\s*)/.Match(first).Groups[0].Value
		return code if 0 == len(indent)
	
		buffer = System.Text.StringBuilder()
		for line in lines:
			if not line.StartsWith(indent):
				return code // let the parser complain about it
			else:
				buffer.Append(line[len(indent):])
				buffer.Append("\n")
		return buffer.ToString()
