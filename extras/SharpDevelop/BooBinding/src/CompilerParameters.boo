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
import System.IO
import System.Text
import System.Xml
import System.Diagnostics
import System.ComponentModel
import ICSharpCode.SharpDevelop.Gui.Components
import ICSharpCode.SharpDevelop.Internal.Project
import ICSharpCode.Core.Services

enum CompileTarget:
	WinExe
	Exe
	Library

enum NetRuntime:
	Mono
	MonoInterpreter
	MsNet

class BooCompilerParameters(AbstractProjectConfiguration):
	_compilerOptions = CompilerOptions()
	
	[Browsable(false)]
	CurrentCompilerOptions:
		get:
			return _compilerOptions
	
	[LocalizedProperty("Output path", Description : "The path where the assembly is created.")]
	OutputPath as string:
		get:
			return OutputDirectory
		set:
			OutputDirectory = value
	
	[LocalizedProperty("Output assembly", Description : "The assembly name.")]
	AssemblyName as string:
		get:
			return OutputAssembly
		set:
			OutputAssembly = value
	
	[LocalizedProperty("Boo path", Description : "The path where the boo compiler and Boo.dll is.")]
	BooPath as string:
		get:
			return _compilerOptions.BooPath
		set:
			_compilerOptions.BooPath = value
			return if value == ""
			propertyService as PropertyService = ServiceManager.Services.GetService(typeof(PropertyService))
			fileUtilityService as FileUtilityService = ServiceManager.Services.GetService(typeof(FileUtilityService))
			booDir = fileUtilityService.GetDirectoryNameWithSeparator(value)
			if File.Exists(booDir + "booc.exe"):
				propertyService.SetProperty("BooBinding.LastBooPath", value)
	
	[LocalizedProperty("Parameters", Description : "Command line parameters passed to the executed application.")]
	CommandLineParameters as string:
		get:
			return _compilerOptions.CommandLineParameters
		set:
			_compilerOptions.CommandLineParameters = value
	
	[DefaultValue(BooBinding.CompileTarget.Exe)]
	[LocalizedProperty("Compile Target", Description : "The compilation target of the source code.")]
	CompileTarget as BooBinding.CompileTarget:
		get:
			return _compilerOptions.CompileTarget
		set:
			_compilerOptions.CompileTarget = value
	
	[DefaultValue(false)]
	[LocalizedProperty("Include debug information", Description : "Specifies if debug information should be omited.")]
	IncludeDebugInformation as bool:
		get:
			return _compilerOptions.IncludeDebugInformation
		set:
			_compilerOptions.IncludeDebugInformation = value
	
	[DefaultValue(true)]
	[LocalizedProperty("Pause console", Description : "Specifies if after the executing the program in the console the window should wait for any key before closing.")]
	PauseConsoleOutput as bool:
		get:
			return _compilerOptions.PauseConsoleOutput
		set:
			_compilerOptions.PauseConsoleOutput = value
	
	[DefaultValue(false)]
	[LocalizedProperty("Verbose", Description : "Specifies if a detailed message should be returned in case of internal compiler errors.")]
	Verbose as bool:
		get:
			return _compilerOptions.Verbose
		set:
			_compilerOptions.Verbose = value
	
	[DefaultValue(BooBinding.NetRuntime.MsNet)]
	[LocalizedProperty("Runtime", Description : "Specifies the runtime for executing the program.")]
	NetRuntime as BooBinding.NetRuntime:
		get:
			return _compilerOptions.NetRuntime
		set:
			_compilerOptions.NetRuntime = value
	
	def constructor(name as string):
		self.name = name
	
	def constructor():
		pass


[XmlNodeName("CompilerOptions")]
class CompilerOptions:
	[XmlAttribute("runtime")]
	public NetRuntime = BooBinding.NetRuntime.MsNet
	
	[XmlAttribute("compileTarget")]
	public CompileTarget = BooBinding.CompileTarget.Exe
	
	[XmlAttribute("includeDebugInformation")]
	public IncludeDebugInformation = false
	
	[XmlAttribute("booPath")]
	public BooPath = ""
	
	[XmlAttribute("commandLineParameters")]
	public CommandLineParameters = ""
	
	[XmlAttribute("pauseConsoleOutput")]
	public PauseConsoleOutput = true
	
	[XmlAttribute("verbose")]
	public Verbose = false
	
	def constructor():
		propertyService as PropertyService = ServiceManager.Services.GetService(typeof(PropertyService))
		self.BooPath = propertyService.GetProperty("BooBinding.LastBooPath", "")
