#region license
// Copyright (c) 2013 Harald Meyer auf'm Hofe (harald_meyer@users.sourceforge.net)
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

namespace Booish.Mod.Os

import System
import System.IO
import System.Diagnostics

import Boo.Lang.Interpreter
import Boo.Lang.Interpreter.ColorScheme

[CmdClass("File")]
class File:
"""Implements Move, Del, Copy commands."""
		
		_interpreter as Interpreter.InteractiveInterpreter

		public def constructor (interpreter as Interpreter.InteractiveInterpreter):
			_interpreter = interpreter
				
		[CmdDeclaration("mv move", Description:"Move a file to another directory or rename it.")]
		public def Move([CmdArgument(CmdArgumentCompletion.File)] src as string,
			[CmdArgument(CmdArgumentCompletion.ExistingOrNotExistingFileOrExistingDirectory)] destination as string):
		"""
			Move a file to another directory or rename it.
		"""
			if System.IO.Directory.Exists(destination):
				destination = Path.Combine(destination, Path.GetFileName(src)) if System.IO.Directory.Exists(destination)
			System.IO.File.Move(src, destination)
		
		[CmdDeclaration("cp copy", Description:"""Copy a file (or more files using wildcards * or ?)
to another directory.
Example:
cp *.boo c:\temp""")]
		public def Copy([CmdArgument(CmdArgumentCompletion.File)] src as string,
			[CmdArgument(CmdArgumentCompletion.ExistingOrNotExistingFileOrExistingDirectory)] destination as string):
		"""
			Copy files to another directory. 
		"""
			if System.IO.Directory.Exists(destination):
				for srcExpanded in Booish.Mod.Os.Directory.EnumerateFiles(src):
					destinationHere = Path.Combine(destination, Path.GetFileName(srcExpanded))
					System.IO.File.Copy(src, destinationHere)
			else:
				System.IO.File.Copy(src, destination)
		
		[CmdDeclaration("rm del", Description: """Delete one or more files. The required
argument may contain wildcards * or ?.
Examples:
delete bullshit.boo
delete *.bak""")]
		public def Delete([CmdArgument(CmdArgumentCompletion.File)] file as string):
		"""
			Delete a file.
		"""
			System.IO.File.Delete(file)
		
		[CmdDeclaration("encrypt", Description: """Encrypt a file. The required
argument may contain wildcards * or ?.
Examples:
encrypt bullshit.boo
encrypt *.bak""")]
		public def Encrypt([CmdArgument(CmdArgumentCompletion.File)] file as string):
		"""
			Encrypt a file.
		"""
			System.IO.File.Encrypt(file)
		
		[CmdDeclaration("decrypt", Description: """Decrypt a file. The required
argument may contain wildcards * or ?.
Examples:
decrypt bullshit.boo
decrypt *.bak""")]
		public def Decrypt([CmdArgument(CmdArgumentCompletion.File)] file as string):
		"""
			Decrypt a file.
		"""
			System.IO.File.Decrypt(file)
		
		[CmdDeclaration("type", Description: "Print the content of a text file.")]
		public def Type([CmdArgument(CmdArgumentCompletion.File)] file as string):
		"""
			Type the content of a file.
		"""
			if file == null:
				WithColor ErrorColor:
					Console.WriteLine("This command requires a path name.")
			else:
				for l in System.IO.File.OpenText(file):
					Console.WriteLine(l)
		
		def FindProgram(filenameWithOrWithoutExt as string) as string:
			if Path.IsPathRooted(filenameWithOrWithoutExt):
				return filenameWithOrWithoutExt
			paths = Environment.GetEnvironmentVariable("path").Split(";"[0])
			for path in paths:
				for fileInPath in System.IO.Directory.GetFiles(path):
					fileNameInPath = Path.GetFileName(fileInPath)
					if string.Equals( fileNameInPath, filenameWithOrWithoutExt, StringComparison.CurrentCultureIgnoreCase)\
						or string.Equals( Path.GetFileNameWithoutExtension(fileNameInPath), filenameWithOrWithoutExt, StringComparison.CurrentCultureIgnoreCase):
							return fileNameInPath
			return filenameWithOrWithoutExt
		
		[CmdDeclaration("run r", Description: "Run an executable file (.boo, .exe, .com, .bat, ...).")]
		public def Run([CmdArgument(CmdArgumentCompletion.File)] file as string):
		"""
			If suffix of the file is .BOO, then run this file within this
			interpreter. Otherwise, pass the file to the system for starting
			a process.
		"""
			file = FindProgram(file)
			if file.EndsWith(".boo", StringComparison.CurrentCultureIgnoreCase):
				using f = System.IO.File.OpenText(file):
					self._interpreter.Eval(f.ReadToEnd())
			else:
				Process.Start(file)
		