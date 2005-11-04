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
	
	_traceLevel = System.Diagnostics.TraceLevel.Off
	
	_rebuild = false
	
	_generateInMemory = false
	
	_debug = true
	
	[TaskAttribute("debug")]
	Debug:
		get:
			return _debug
		set:
			_debug = value
	
	[BuildElement("rebuild")]
	Rebuild:
		get:
			return _rebuild
		set:
			_rebuild = value
	
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
			
	[TaskAttribute("generateInMemory")]
	GenerateInMemory:
		get:
			return _generateInMemory
		set:
			_generateInMemory = false
			
	[BuildElement("sources", Required: true)]
	Sources:
		get:
			return _sourceFiles
		set:
			_sourceFiles = value
	
	override def ExecuteTask():
		return unless NeedsCompiling()
		
		files = _sourceFiles.FileNames
		LogInfo("Compiling ${len(files)} file(s) to ${_output}.")
		
		if _traceLevel != TraceLevel.Off:
			Trace.Listeners.Add(TextWriterTraceListener(System.Console.Out))
		
		compiler = BooCompiler()
		parameters = compiler.Parameters
		parameters.TraceSwitch.Level = _traceLevel
		parameters.OutputAssembly = _output.ToString()
		parameters.OutputType = GetOutputType()
		parameters.GenerateInMemory = _generateInMemory
		parameters.Debug = _debug
		
		for fname as string in files:
			print("source: ${fname}")
			parameters.Input.Add(FileInput(fname))
			
		for fname as string in _resources.FileNames:
			LogVerbose(fname)
			parameters.Resources.Add(FileResource(fname))
			
		RunCompiler(compiler)
		
	override def GetDefaultPipeline():
		return Boo.Lang.Compiler.Pipelines.CompileToFile()

	private def GetOutputType():
		if "exe" == _target:
			return CompilerOutputType.ConsoleApplication
		else:
			if "winexe" == _target:
				return CompilerOutputType.WindowsApplication
		return CompilerOutputType.Library
		
	private def NeedsCompiling():
		if _rebuild:
			LogVerbose("rebuild requested.")
			return true
			
		if not _output.Exists:
			LogVerbose("${_output} does not exist.")
			return true
		return (
			HasMoreRecentFile(_sourceFiles) or
			HasMoreRecentFile(_references) or
			HasMoreRecentFile(_resources))
			
	def HasMoreRecentFile(fs as FileSet):
		found = FileSet.FindMoreRecentLastWriteTime(fs.FileNames, _output.LastWriteTime)
		if found is not null:
			LogVerbose("${found} is newer than ${_output}.")
			return true


