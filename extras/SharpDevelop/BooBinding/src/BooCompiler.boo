#region license
// Copyright (c) 2004, Daniel Grunwald (daniel@danielgrunwald.de)
// All rights reserved.
//
// BooBinding is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Boo Explorer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

namespace BooBinding

import System
import System.Collections
import System.Diagnostics
import System.IO
import System.Globalization
import System.Threading
import System.Reflection.Assembly as Assembly
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Resources

class BooCompilerWrapper(MarshalByRefObject):
	_compiler as BooCompiler
	_options as CompilerParameters
	_defaultReferences = ("System.Data",
	                      "System.Drawing",
	                      "System.Management",
	                      "System.Messaging",
	                      "System.Runtime.Remoting",
	                      "System.Runtime.Serialization.Formatters.Soap",
	                      "System.Security",
	                      "System.ServiceProcess",
	                      "System.Web",
	                      "System.Web.Services",
	                      "System.Windows.Forms",
	                      "System.Xml")
	
	def constructor():
		_compiler = BooCompiler()
		_options = _compiler.Parameters
		for reference in _defaultReferences:
			AddReference(reference)
	
	def SetOptions(o as BooBinding.CompilerOptions):
		/* This code creates an internal compiler error when compiling with the NAnt build file
		_options.Debug = o.IncludeDebugInformation
		
		if o.CompileTarget == CompileTarget.WinExe:
			_options.OutputType = CompilerOutputType.WindowsApplication
		elif o.CompileTarget == CompileTarget.Library:
			_options.OutputType = CompilerOutputType.Library
		else:
			_options.OutputType = CompilerOutputType.ConsoleApplication
		*/
		_options.Pipeline = CompileToFile() if _options.Pipeline == null
	
	OutputFile as string:
		get:
			return _options.OutputAssembly
		set:
			_options.OutputAssembly = value
	
	def AddInputFile(fileName as string):
		_options.Input.Add(FileInput(fileName))
	
	def AddReference(assemblyName as string):
		reference as Assembly = Assembly.LoadWithPartialName(assemblyName)
		if reference == null:
			reference = Assembly.LoadFrom(Path.GetFullPath(assemblyName))
			if reference == null:
				raise ApplicationException('Unable to load reference ' + assemblyName)
		
		_options.References.Add(reference)
	
	def AddResource(fileName as string):
		_options.Resources.Add(FileResource(fileName))
	
	def Run():
		context = _compiler.Run()
		result = ArrayList()
		for error as CompilerError in context.Errors:
			result.Add(error)
		for warning as CompilerWarning in context.Warnings:
			result.Add(warning)
		return result

