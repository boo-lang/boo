namespace Boo.NAnt

import System.Diagnostics
import System.IO
import NAnt.Core
import NAnt.Core.Attributes
import NAnt.Core.Types
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO

[TaskName("booc")]
class BooC(Task):
	
	_output as FileInfo
	
	_target = "exe"
	
	_sourceFiles = FileSet()
	
	_references = FileSet()
	
	_pipeline = "booc"
	
	_traceLevel = System.Diagnostics.TraceLevel.Off
	
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
			
	[BuildElement("references")]
	References:
		get:
			return _references
		set:
			_references = value
			
	[TaskAttribute("pipeline")]
	Pipeline:
		get:
			return _pipeline
		set:
			_pipeline = value
			
	protected def ExecuteTask():
		files = _sourceFiles.FileNames
		LogInfo("Compiling ${files.Count} files to ${_output}.")
		
		compiler = BooCompiler()
		parameters = compiler.Parameters
		parameters.TraceSwitch.Level = _traceLevel
		parameters.OutputAssembly = _output.ToString()
		parameters.OutputType = GetOutputType()
		parameters.Pipeline.Load(Project.BaseDirectory, _pipeline)
		
		for fname as string in files:
			LogVerbose(fname)
			parameters.Input.Add(FileInput(fname))
			
		AddReferences(parameters)		
			
		context = compiler.Run()
		errors = context.Errors
		for error as CompilerError in errors:
			LogError(error.ToString(parameters.TraceSwitch.TraceInfo))
			
		if errors.Count:
			LogInfo("${errors.Count} error(s).")
			raise BuildException("boo compilation error", Location)
		
	private def AddReferences(parameters as CompilerParameters):
		
		baseDir = _references.BaseDirectory.ToString()
		frameworkDir = Project.CurrentFramework.FrameworkAssemblyDirectory.ToString()
		for reference as string in _references.Includes:
			
			path = reference
			if not Path.IsPathRooted(path):
				path = Path.Combine(baseDir, reference)
				if not File.Exists(path):
					path = Path.Combine(frameworkDir, reference)
					
			LogVerbose(path)		
			try:
				parameters.References.Add(System.Reflection.Assembly.LoadFrom(path))
			except x:
				raise BuildException(
					Boo.ResourceManager.Format("BooC.UnableToLoadAssembly", reference),
					Location,
					x)					

	private def GetOutputType():
		if "exe" == _target:
			return CompilerOutputType.ConsoleApplication
		else:
			if "winexe" == _target:
				return CompilerOutputType.WindowsApplication
		return CompilerOutputType.Library
		
	private def LogInfo(message as string):
		self.Log(Level.Info, "${LogPrefix}${message}")
		
	private def LogVerbose(message as string):
		self.Log(Level.Verbose, "${LogPrefix}${message}")
		
	private def LogError(message as string):
		self.Log(Level.Error, "${LogPrefix}${message}")
	
