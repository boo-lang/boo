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

import System.Diagnostics
import System.IO
import NAnt.Core
import NAnt.Core.Attributes
import NAnt.Core.Types
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Resources

[TaskName("booc")]
class BoocTask(AbstractBooTask):
	
	_output as FileInfo
	
	_target = "exe"
	
	_sourceFiles = FileSet()
	
	_resources = FileSet()
	
	_pipeline as string
	
	_traceLevel = System.Diagnostics.TraceLevel.Off
	
	_references = FileSet()
	
	[BuildElement("references")]
	References:
		get:
			return _references
		set:
			_references = value
	
	[BuildElement("resources")]
	Resources:
		get:
			return _resources
		set:
			_resources = value
	
	[TaskAttribute("output", Required: true)]
	Output:
		get:
			return _output
		set:
			_output = value
			
	[TaskAttribute("tracelevel")]
	TraceLevel:
		get:
			return _traceLevel
			
		set:
			_traceLevel = value
			
	[TaskAttribute("target")]
	Target:
		get:
			return _target
		set:
			if value not in ("exe", "winexe", "library"):
				raise BuildException(
						"target must be one of: exe, winexe, library",
						Location)
			_target = value
			
	[BuildElement("sources", Required: true)]
	Sources:
		get:
			return _sourceFiles
		set:
			_sourceFiles = value
			
	[TaskAttribute("pipeline")]
	Pipeline:
		get:
			return _pipeline
		set:
			_pipeline = value
			
	override protected def ExecuteTask():
		files = _sourceFiles.FileNames
		LogInfo("Compiling ${len(files)} file(s) to ${_output}.")
		
		compiler = BooCompiler()
		parameters = compiler.Parameters
		parameters.TraceSwitch.Level = _traceLevel
		parameters.OutputAssembly = _output.ToString()
		parameters.OutputType = GetOutputType()
		if _pipeline:
			parameters.Pipeline = CompilerPipeline.GetPipeline(_pipeline)
		else:
			parameters.Pipeline = Boo.Lang.Compiler.Pipelines.CompileToFile()
		
		for fname as string in files:
			print("source: ${fname}")
			parameters.Input.Add(FileInput(fname))
			
		for fname as string in _resources.FileNames:
			LogVerbose(fname)
			parameters.Resources.Add(FileResource(fname))
			
		AddReferences(parameters)		
			
		RunCompiler(compiler)
		
		
	protected def AddReferences(parameters as CompilerParameters):
		
		if _references.BaseDirectory is not null:
			baseDir = _references.BaseDirectory.ToString()
		else:
			baseDir = Project.BaseDirectory
			
		frameworkDir = GetFrameworkDirectory()
		for reference as string in _references.Includes:
			
			path = reference
			if not Path.IsPathRooted(path):
				path = Path.Combine(baseDir, reference)
				if not File.Exists(path):
					print("${path} doesn't exist.")
					path = Path.Combine(frameworkDir, reference)
					
			print("reference: ${path}")		
			try:
				parameters.References.Add(System.Reflection.Assembly.LoadFrom(path))
			except x:
				raise BuildException(
					Boo.ResourceManager.Format("BCE0041", reference),
					Location,
					x)

	private def GetOutputType():
		if "exe" == _target:
			return CompilerOutputType.ConsoleApplication
		else:
			if "winexe" == _target:
				return CompilerOutputType.WindowsApplication
		return CompilerOutputType.Library
