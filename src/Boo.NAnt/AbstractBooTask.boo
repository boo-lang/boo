#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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

abstract class AbstractBooTask(Task):
	
	_references = FileSet()
	
	[BuildElement("references")]
	References:
		get:
			return _references
		set:
			_references = value

	protected def AddReferences(parameters as CompilerParameters):
		
		if _references.BaseDirectory is not null:
			baseDir = _references.BaseDirectory.ToString()
		else:
			baseDir = Project.BaseDirectory
			
		frameworkDir = Project.TargetFramework.FrameworkAssemblyDirectory.ToString()
		for reference as string in _references.Includes:
			
			path = reference
			if not Path.IsPathRooted(path):
				path = Path.Combine(baseDir, reference)
				if not File.Exists(path):
					self.LogVerbose("${path} doesn't exist.")
					path = Path.Combine(frameworkDir, reference)
					
			LogVerbose(path)		
			try:
				parameters.References.Add(System.Reflection.Assembly.LoadFrom(path))
			except x:
				raise BuildException(
					Boo.ResourceManager.Format("BCE0041", reference),
					Location,
					x)

	protected def LogInfo(message as string):
		self.Log(Level.Info, "${LogPrefix}${message}")
		
	protected def LogVerbose(message as string):
		self.Log(Level.Verbose, "${LogPrefix}${message}")
		
	protected def LogError(message as string):
		self.Log(Level.Error, "${LogPrefix}${message}")
