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
import System.Collections
import System.Diagnostics
import System.IO
import System.Globalization
import System.Text
import System.Threading
import System.Reflection.Assembly as Assembly
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Resources

class BooCompilerWrapper:
	
	[property(OutputFile)]
	_outputFile as string
	_references = []
	_resources = []
	_inputFiles = []
	_options as BooBinding.CompilerOptions	
	
	def SetOptions(o as BooBinding.CompilerOptions):
		_options = o
	
	def AddInputFile(fileName as string):
		_inputFiles.Add(fileName)
	
	def AddReference(assemblyName as string):
		_references.Add(assemblyName)
	
	def AddResource(fileName as string):
		_resources.Add(fileName)
	
	def Run():
		args = []
		
		if _options.CompileTarget == CompileTarget.WinExe:
			args.Add("-t:winexe")
		elif _options.CompileTarget == CompileTarget.Library:
			args.Add("-t:library")
		
		if _options.DuckTypingByDefault:
			args.Add("-ducky")
		
		args.Add("-o:${OutputFile}")		
		for fname in _references:
			args.Add("-r:${fname}")
		for fname in _resources:
			args.Add("-resource:${fname}")
		for fname in _inputFiles:
			args.Add(fname)
			
		// shellm executes the compiler inprocess in a new AppDomain
		// for some reason, the compiler output sometimes contains
		// spurious messages from the main AppDomain 
		
		//return shellm(GetBoocLocation(), args.ToArray(string))
		// Switching to shell instead of shellm because of BOO-243
		return shell(GetBoocLocation(), join("\"${arg}\"" for arg in args))
		
	def GetBoocLocation():
		return Path.Combine(
			Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
			"booc.exe")
