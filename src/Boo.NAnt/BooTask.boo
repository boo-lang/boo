#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
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
		parameters.References.Add(GetType().Assembly)
		parameters.References.Add(typeof(NAnt.Core.Project).Assembly)
		
		result = RunCompiler(compiler)		

		print("script successfully compiled.")
		
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
