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

namespace Boo.NAnt

import System
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
	
	Project:
		get:
			return _project
		set:
			_project = value
	
	_project as Project
	
	Task:
		get:
			return _task
		set:
			_task = value
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
		script.Members.Extend(module.Members)
		
		module.Members.Clear()
		module.Members.Add(script)		
		
def WithWorkingDir(dir as string, block as callable()):
	_saved = Environment.CurrentDirectory
	Environment.CurrentDirectory = dir
	try:
		block()
	ensure:	
		Environment.CurrentDirectory = _saved

[TaskName("boo")]
class BooTask(AbstractBooTask):

	_src as FileInfo
	_code as RawXml
	
	[TaskAttribute("src", Required: false)]
	Source:
		get: return _src
		set: _src = value
			
	[BuildElement("code")]
	Code as RawXml:
		get: return _code
		set: _code = value
			
	override def ExecuteTask():
		
		compiler = BooCompiler()
		parameters = compiler.Parameters
		parameters.OutputType = CompilerOutputType.Library
		if _src:
			parameters.Input.Add(FileInput(_src.ToString()))
		else:
			parameters.Input.Add(StringInput("boo", reindent(getSourceCode())))
		parameters.References.Add(GetType().Assembly)
		parameters.References.Add(typeof(NAnt.Core.Project).Assembly)
		
		result = RunCompiler(compiler)		

		print("script successfully compiled.")
		try:
			scriptType = result.GeneratedAssembly.GetType("__Script__", true)
			script as AbstractScript = scriptType()
			script.Project = Project
			script.Task = self
			WithWorkingDir(Project.BaseDirectory) do:
				script.Run()
		except x:
			raise BuildException(x.Message, Location, x)
			
	override def GetDefaultPipeline():
		pipeline = CompileToMemory()
		pipeline.Insert(1, PrepareScriptStep())
		return pipeline
			
	private def getSourceCode():
		if Code is not null:
			return Code.Xml.InnerText
		return XmlNode.InnerText
			
	private def reindent(code as string):
		lines = /\n/.Split(code.Replace("\r\n", "\n"))
		lines = array(line for line in lines if len(line.Trim()))
	
		first = lines[0]
		indent = /(\s*)/.Match(first).Groups[0].Value
		return code if 0 == len(indent)
	
		buffer = System.Text.StringBuilder()
		for line in lines:
			if not line.StartsWith(indent):
				return code // let the parser complain about it
			
			buffer.Append(line[len(indent):])
			buffer.Append("\n")
		return buffer.ToString()
