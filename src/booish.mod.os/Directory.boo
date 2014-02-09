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
import System.IO.Directory

import Boo.Lang.Interpreter
import Boo.Lang.Interpreter.ColorScheme

[CmdClass("Directory")]
class Directory:
"""Implements CD, DIR, LS and PWD commands."""
	
	final static CONFIG_FILENAME = "booish\\Shell\\DirectoryConfig.ini"	
	
	static ConfigFileName as string:
		get:
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CONFIG_FILENAME)
	
	def constructor():
		configFileName=ConfigFileName
		if System.IO.File.Exists(configFileName):
			for l in System.IO.File.OpenText(configFileName):
				s=l.Split(*':=, '.ToCharArray())
				self.GetType().GetMethod(s[0]).Invoke(self, (s[1],))
	
	_dirStack = []
	"""A list representing the stack of visited directories."""
	_dirStackIndex=-1
		
	[CmdDeclaration("cd chdir", Description:"Change the current working directory.")]
	public def Cd([CmdArgument(CmdArgumentCompletion.Directory, DefaultValue:"")] newDir):
	"""Change current directory."""
		if len(_dirStack) == 0:		
			_dirStack.Add(GetCurrentDirectory())
			_dirStackIndex = 0
		if string.IsNullOrEmpty(newDir):
			newDir=Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
		newDir=Environment.ExpandEnvironmentVariables(newDir)
		_dirStack.RemoveAt(len(_dirStack)-1) while len(_dirStack)-1 > _dirStackIndex 
		_dirStack.Add(newDir)
		_dirStack.RemoveAt(0) while len(_dirStack) > 100
		_dirStackIndex = len(_dirStack)-1
		SetCurrentDirectory(newDir)
		Console.WriteLine(GetCurrentDirectory())
	
	[CmdDeclaration("pwd workdir", Description:"Show the current working directory.")]
	public def Pwd():
	"""Show the current working directory."""
		Console.WriteLine(GetCurrentDirectory())
	
	[CmdDeclaration("cdb chdirback", Description:"Go back to the directory that you visited before.")]
	public def Cdb():
		if _dirStackIndex <= 0:
			WithColor(ErrorColor):
				print "I do not know where to go to."
		else:
			_dirStackIndex -= 1
			GotoStackIndexDir()
	
	[CmdDeclaration("cdf chdirforward", Description:"Go forward to the directory that yoo left by CHDIRBACK.")]
	public def Cdf():
		if _dirStackIndex+1 >= len(_dirStack):
			WithColor(ErrorColor):
				print "I do not know where to go to."
		else:
			_dirStackIndex += 1
			GotoStackIndexDir()
	
	def GotoStackIndexDir():
		SetCurrentDirectory(_dirStack[_dirStackIndex])
		Console.WriteLine(_dirStack[_dirStackIndex])
		
	[CmdDeclaration("dir ls", Description: "Show files and directories.")]
	public def Dir([CmdArgument(CmdArgumentCompletion.Directory, DefaultValue:".")] directory):
		LsDir(directory)
		LsFile(directory)
	
	_widthCreationDate=0
	[CmdDeclaration("SetWidthCreationDate", Description: "Defines the width of the column showing the creation date.")]
	public def SetWidthCreationDate([CmdArgument(CmdArgumentCompletion.None, DefaultValue:"-1")] width):
		iWidth = Convert.ToInt32(width)
		if iWidth < 0:
			Console.WriteLine("SetWidthCreationDate "+self._widthCreationDate.ToString())
		else:
			_widthCreationDate= iWidth
			WriteConfig()
	
	def WriteConfig():
		System.IO.Directory.CreateDirectory(Path.GetDirectoryName(ConfigFileName))
		using f=System.IO.File.CreateText(ConfigFileName):
			f.WriteLine("SetWidthCreationDate "+self._widthCreationDate.ToString())

	[CmdDeclaration("lsdir", Description: "Show directories.")]
	public def LsDir([CmdArgument(CmdArgumentCompletion.Directory, DefaultValue:".")] directory):
		for d in System.IO.Directory.GetDirectories(directory):
			if self._widthCreationDate > 0:
				Console.Write(System.IO.Directory.GetCreationTime(d).ToString("g")[:self._widthCreationDate].PadLeft(self._widthCreationDate)+' ')
			Console.WriteLine(d+"\\")
	
	[CmdDeclaration("lsfiles", Description: "Show files.")]
	public def LsFile([CmdArgument(CmdArgumentCompletion.Directory, DefaultValue:".")] directory):
		for f in System.IO.Directory.GetFiles(directory):
			if self._widthCreationDate > 0:
				Console.Write(System.IO.File.GetCreationTime(f).ToString("g")[:self._widthCreationDate].PadLeft(self._widthCreationDate)+' ')
			Console.WriteLine(f)

