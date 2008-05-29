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

namespace Boo.Microsoft.Build.Tasks

import System
import System.IO
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Pipelines
import Microsoft.Build.Framework
import Microsoft.Build.Tasks
import Microsoft.Build.Utilities

class ExecBoo(Task):
	[property(Source,Attributes:[Required])] _src as string
	[property(Arguments)] _args as (string)
	[property(References)] _references as (string)
	[property(Pipeline)] _pipeline as string
	[property(Ducky)] _ducky = false
	[property(WhiteSpaceAgnostic)] _wsa = false
	[property(ScriptResult,Attributes:[Output])] _scriptResult as (string)

	_buildSuccess = true

	def WithWorkingDir(dir as string, block as callable()):
		_saved = Environment.CurrentDirectory
		Environment.CurrentDirectory = dir
		try:
			block()
		ensure:	
			Environment.CurrentDirectory = _saved

	def Execute():
		compiler = BooCompiler()
		parameters = compiler.Parameters
		parameters.Ducky = Ducky
		parameters.WhiteSpaceAgnostic = WhiteSpaceAgnostic
		parameters.OutputType = CompilerOutputType.Library
		parameters.Input.Add(FileInput(_src.ToString()))
		parameters.References.Add(GetType().Assembly)
		parameters.References.Add(typeof(Microsoft.Build.Utilities.Task).Assembly)
		
		result = RunCompiler(compiler)
		
		return false unless _buildSuccess

#		print("script successfully compiled.")
		try:
			scriptType = result.GeneratedAssembly.GetType("__Script__", true)
			script as AbstractScript = scriptType()
			script.Task = self
			WithWorkingDir(Path.GetDirectoryName(script.Task.BuildEngine.ProjectFileOfTaskNode)) do:
				script.Run()
			_buildSuccess = script.Success
		except x:
#			Log.LogErrorFromException(x, true, true, _src)
			Log.LogErrorFromException(x, true)
			_buildSuccess = false

		return _buildSuccess
		
	protected def RunCompiler(compiler as BooCompiler):
		AddReferences(compiler.Parameters)
		if _pipeline:
			compiler.Parameters.Pipeline = CompilerPipeline.GetPipeline(_pipeline)
		else:
			compiler.Parameters.Pipeline = GetDefaultPipeline()				
		result = compiler.Run()
		CheckCompilationResult(result)
		return result
			
	protected def AddReferences(parameters as CompilerParameters):
		return unless _references
		baseDir = Path.GetDirectoryName(BuildEngine.ProjectFileOfTaskNode)
		for reference in _references:
			path = reference
			if not Path.IsPathRooted(path):
				path = Path.Combine(baseDir, reference)
				if not File.Exists(path):
#					Log.LogMessage("${path} doesn't exist.")
					asm = Reflection.Assembly.Load(Path.GetFileNameWithoutExtension(reference))
				else:
					asm = Reflection.Assembly.LoadFrom(path)
			else:
				asm = Reflection.Assembly.LoadFrom(path)

#			print("reference: ${path}")
			try:
				parameters.References.Add(asm)
				//TODO: rationalize above call to parameters.References.Add(parameters.LoadAssembly(reference)) ?
				//      move basedir into CompilerParameters ?
			except x:
#				Log.LogErrorFromException(x, true, true, path)
				Log.LogErrorFromException(x, true)
					
	protected def CheckCompilationResult(context as CompilerContext):
		errors = context.Errors
		verbose = context.Parameters.TraceInfo
		for error in errors:
			Log.LogError(error.ToString(verbose))
		for warning in context.Warnings:
			Log.LogWarning(warning.ToString());
			
		if len(errors):
			_buildSuccess = false
			Log.LogMessage("${len(errors)} error(s).")

	def GetFrameworkDirectory():
		return Microsoft.Build.Tasks.GetFrameworkPath().Path
			
	def GetDefaultPipeline():
		pipeline = CompileToMemory()
		pipeline.Insert(1, PrepareScriptStep())
		return pipeline
