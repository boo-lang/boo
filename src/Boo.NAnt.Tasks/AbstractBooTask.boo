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
import System.Text
import System.IO
import NAnt.Core
import NAnt.Core.Attributes
import NAnt.Core.Types
import Boo.Lang.Compiler

abstract class AbstractBooTask(Task):
	
	_references = FileSet()
	
	_packages = OptionCollection()
	
	_pipeline as string
	
	[BuildElement("references")]
	References:
		get:
			return _references
		set:
			_references = value			
		
	[BuildElementCollection("pkg-references", "package")]
	Packages:
		get:
			return _packages
		set:
			_packages = value
			
	[TaskAttribute("pipeline")]
	Pipeline:
		get:
			return _pipeline
		set:
			_pipeline = value
	
	protected def RunCompiler(compiler as BooCompiler):
		AddReferences(compiler.Parameters)
		AddPackages(compiler.Parameters)
		if _pipeline:
			compiler.Parameters.Pipeline = CompilerPipeline.GetPipeline(_pipeline)
		else:
			compiler.Parameters.Pipeline = GetDefaultPipeline()				
		result = compiler.Run()
		CheckCompilationResult(result)
		return result
			
	protected def AddReferences(parameters as CompilerParameters):
		if _references.BaseDirectory is not null:
			baseDir = _references.BaseDirectory.ToString()
		else:
			baseDir = Project.BaseDirectory
		
		for reference as string in _references.Includes:
			path = reference
			if not Path.IsPathRooted(path):
				path = Path.Combine(baseDir, reference)
				if not File.Exists(path):
					print("${path} doesn't exist.")
					asm = Reflection.Assembly.Load(Path.GetFileNameWithoutExtension(reference))
				else:
					asm = Reflection.Assembly.LoadFrom(path)
			else:
				asm = Reflection.Assembly.LoadFrom(path)

			print("reference: ${path}")
			try:
				parameters.References.Add(asm)
				//TODO: rationalize above call to parameters.References.Add(parameters.LoadAssembly(reference)) ?
				//      move basedir into CompilerParameters ?
			except x:
				raise BuildException(
					Boo.Lang.ResourceManager.Format("BCE0041", reference),
					Location,
					x)
					
	protected def AddPackages(parameters as CompilerParameters):
		for package as string in _packages:
			try:
				parameters.LoadReferencesFromPackage(package);
			except x:
				raise BuildException(
					Boo.Lang.ResourceManager.Format("BCE0041", package),
					Location,
					x)
	
	protected def CheckCompilationResult(context as CompilerContext):
		errors = context.Errors
		verbose = context.Parameters.TraceInfo
		for error in errors:
			LogError(error.ToString(verbose))
		for warning in context.Warnings:
			LogWarning(warning.ToString());
			
		if len(errors):
			LogInfo("${len(errors)} error(s).")
			raise BuildException("boo compilation error", Location)
			
	abstract def GetDefaultPipeline() as CompilerPipeline:
		pass

	def GetFrameworkDirectory():
		return Project.TargetFramework.FrameworkAssemblyDirectory.ToString()
		
	def print(message):
		LogVerbose(message)

	def LogInfo(message):
		self.Log(Level.Info, "${message}")
		
	def LogVerbose(message):
		self.Log(Level.Verbose, "${message}")
		
	def LogWarning(message):
		self.Log(Level.Warning, "${message}")
		
	def LogError(message):
		self.Log(Level.Error, "${message}")
		
def read(fname as string):
	try:
		reader=File.OpenText(fname)
		return reader.ReadToEnd()
	ensure:
		reader.Dispose() unless reader is null
	
def write(fname as string, contents as string):
	try:
		writer=StreamWriter(fname, false, System.Text.Encoding.UTF8)
		writer.Write(contents)
	ensure:
		writer.Dispose() unless writer is null

