namespace Boo.NAnt

using System.IO
using NAnt.Core
using NAnt.Core.Attributes
using NAnt.Core.Types
using Boo.Lang.Compiler

[TaskName("booc")]
class BooC(Task):
	
	_output as FileInfo
	
	_target as string
	
	_sourceFiles as FileSet
	
	_references as FileSet
	
	def constructor():
		_sourceFiles = FileSet()
		_references = FileSet()
		_target = "exe"
	
	[TaskAttribute("output", Required: true)]
	Output:
		get:
			return _output
		set:
			_output = value
			
	[TaskAttribute("target")]
	Target:
		get:
			return _target
		set:
			if not value in ("exe", "winexe", "library"):
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
			references = value
			
	protected def ExecuteTask():
		files = _sourceFiles.FileNames
		LogInfo("Compiling ${files.Count} files to ${_output}.")
		for fname in files:
			LogInfo("    ${fname}")
		LogInfo("done!")
		
	private def LogInfo(message as string):
		self.Log(Level.Info, message)
	
