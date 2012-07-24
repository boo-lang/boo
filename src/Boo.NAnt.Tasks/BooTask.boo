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
import System.Linq.Enumerable
import NAnt.Core
import NAnt.Core.Attributes
import NAnt.Core.Types
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps

class ScriptBase:
	
	public Project as Project
	
	public Task as BooTask
	
	def print(msg):
		Task.LogInfo(msg)
	
	abstract def Run(argv as (string)):
		pass

class PrepareScriptStep(AbstractCompilerStep):
	
	override def Run():
		module = CompileUnit.Modules[0]
		
		script = ClassDefinition(Name: "__Script__")
		script.BaseTypes.Add(SimpleTypeReference("Boo.NAnt.ScriptBase"))
		script.Members.Add(RunMethodFor(module))
		script.Members.AddRange(module.Members)
		
		module.Members.Clear()
		module.Members.Add(script)
		module.Globals = Block()
		
	def RunMethodFor(module as Module):	
		return [|
			override def Run(argv as (string)):
				$(RunMethodBodyFor(module))
		|]
		
	def RunMethodBodyFor(module as Module) as Statement:
		mainMethod = module.Members["Main"] as Method 
		if mainMethod is not null:
			if not module.Globals.IsEmpty: raise "Either provide a Main method or global statements but not both!"
			mainInvocation = ([| Main(argv) |] if len(mainMethod.Parameters) > 0 else [| Main() |])
			return ExpressionStatement(mainInvocation)
		return module.Globals
		
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
	_arguments = ArgumentCollection() 
	
	[TaskAttribute("src", Required: false)]
	Source:
		get: return _src
		set: _src = value
			
	[BuildElement("code")]
	Code as RawXml:
		get: return _code
		set: _code = value
		
	[BuildElementArray("arg")]
	Arguments as ArgumentCollection:
		get: return _arguments
			
	override def ExecuteTask():
		
		compiler = BooCompiler()
		parameters = compiler.Parameters
		parameters.OutputType = CompilerOutputType.Library
		if _src:
			parameters.Input.Add(FileInput(_src.ToString()))
		else:
			parameters.Input.Add(StringInput("code", ReIndent(SourceCode())))
		parameters.References.Add(GetType().Assembly)
		parameters.References.Add(typeof(NAnt.Core.Project).Assembly)
		
		result = RunCompiler(compiler)		

		print("script successfully compiled.")
		try:
			scriptType = result.GeneratedAssembly.GetType("__Script__", true)
			script as ScriptBase = scriptType()
			script.Project = Project
			script.Task = self
			WithWorkingDir(Project.BaseDirectory) do:
				script.Run([arg.ToString() for arg in Arguments].ToArray(string))
		except x:
			raise BuildException(x.Message, Location, x)
			
	override def GetDefaultPipeline():
		pipeline = CompileToMemory()
		pipeline.Insert(1, PrepareScriptStep())
		return pipeline
			
	private def SourceCode():
		if Code is not null:
			return Code.Xml.InnerText
		return XmlNode.InnerText
			

def ReIndent(code as string):	
	lines = code.Replace("\r\n", "\n").Split(char('\n'))
	nonEmptyLines = line for line in lines if len(line.Trim())

	indentation = /(\s*)/.Match(nonEmptyLines.First()).Groups[0].Value
	return code if len(indentation) == 0

	buffer = System.Text.StringBuilder()
	for line in lines:
		if line.StartsWith(indentation):
			buffer.AppendLine(line[len(indentation):])
		else:
			buffer.AppendLine(line)
	return buffer.ToString()
