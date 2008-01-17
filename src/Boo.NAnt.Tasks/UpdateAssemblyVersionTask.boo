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

import System.Text.RegularExpressions
import NAnt.Core
import NAnt.Core.Attributes
import NAnt.Core.Types

[TaskName("updateAssemblyVersion")]
class UpdateAssemblyVersionTask(Task):
	
	_fileSets as (FileSet)
	
	_version as string
	
	_copyright as string
	
	[BuildElementArray("fileset", Required: true)]
	FileSets:
		get:
			return _fileSets
		set:
			_fileSets = value
			
	[TaskAttribute("version", Required: false)]
	Version:
		get:
			return _version
		set:
			_version = value
			
	[TaskAttribute("copyright", Required: false)]
	Copyright:
		get:
			return _copyright
		set:
			_copyright = value
			
	override def ExecuteTask():
		for fileSet in _fileSets:
			for fname in fileSet.FileNames:		
				Update(fname)
		
	def Update(fname as string):
		if _version is null and _copyright is null:
			raise BuildException(
					"At least one of the attributes 'copyright' or 'version' must be set",
					self.Location)
		contents = read(fname)
		if _version is not null:
			newContents = /AssemblyVersion\((.+)\)/.Replace(contents) do (m as Match):
				return m.Groups[0].Value.Replace(m.Groups[1].Value, "\"${_version}\"")
		if _copyright is not null:
			newContents = /AssemblyCopyright\((.+)\)/.Replace(contents) do (m as Match):
				return m.Groups[0].Value.Replace(m.Groups[1].Value, "\"${_copyright}\"")
		if newContents != contents:
			print(fname)
			write(fname, newContents)
			
	def print(message):
		self.Log(Level.Info, "${message}")
