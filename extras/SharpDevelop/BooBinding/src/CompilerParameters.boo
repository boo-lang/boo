#region license
// Copyright (c) 2004, Daniel Grunwald (daniel@danielgrunwald.de)
// All rights reserved.
//
// BooBinding is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// BooBinding is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with BooBinding; if not, write to the Free Software
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
	
	[DefaultValue(NetRuntime.MsNet)]
	[LocalizedProperty("Runtime", Description : "Specifies the runtime for executing the program.")]
	Runtime as NetRuntime:
		get:
			return _compilerOptions.Runtime
		set:
			_compilerOptions.Runtime = value
	
	[DefaultValue(false)]
	[LocalizedProperty("Duck typing by default", Description : "A slower but more flexible python-like mode in which types that cannot be inferred are resolved at runtime (duck typed).")]
	DuckTypingByDefault as bool:
		get:
			return _compilerOptions.DuckTypingByDefault
		set:
			_compilerOptions.DuckTypingByDefault = value
	
	def constructor(name as string):
		self.name = name
	
	def constructor():
		pass


[XmlNodeName("CompilerOptions")]
class CompilerOptions:
	[XmlAttribute("runtime")]
	public Runtime = NetRuntime.MsNet
	
	[XmlAttribute("compileTarget")]
	public CompileTarget = BooBinding.CompileTarget.Exe
	
	[XmlAttribute("includeDebugInformation")]
	public IncludeDebugInformation = false
	
	[XmlAttribute("commandLineParameters")]
	public CommandLineParameters = ""
	
	[XmlAttribute("pauseConsoleOutput")]
	public PauseConsoleOutput = true
	
	[XmlAttribute("duckTypingByDefault")]
	public DuckTypingByDefault = false
	
