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
			parameters.Pipeline = GetPipeline(_pipeline)
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
					
	private def GetPipeline(pipeline as string):
		type = System.Type.GetType(pipeline, true)
		return type()

	private def GetOutputType():
		if "exe" == _target:
			return CompilerOutputType.ConsoleApplication
		else:
			if "winexe" == _target:
				return CompilerOutputType.WindowsApplication
		return CompilerOutputType.Library
