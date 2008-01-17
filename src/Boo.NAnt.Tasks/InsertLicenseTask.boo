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
"""
Makes sure that every file in the fileset starts
with the license notice.
"""

namespace Boo.NAnt

import System.IO
import NAnt.Core
import NAnt.Core.Attributes
import NAnt.Core.Types

[TaskName("insertLicense")]
class InsertLicenseTask(Task):
	
	_fileSets as (FileSet)
	
	_license as FileInfo
	
	[BuildElementArray("fileset", Required: true)]
	FileSets:
		get:
			return _fileSets
		set:
			_fileSets = value
			
	[TaskAttribute("license", Required: true)]
	License:
		get:
			return _license
		set:
			_license = value
			
	override def ExecuteTask():
		licenseText = read(_license.FullName)
		for fileSet in _fileSets:
			for fname in fileSet.FileNames:		
				InsertLicense(fname, licenseText) if GetFirstLine(fname) != "#region license"
		
	def InsertLicense(fname as string, license as string):
		print(fname)
		contents = read(fname)
		try:
			writer=StreamWriter(fname, false, System.Text.Encoding.UTF8)
			writer.WriteLine(license)
			writer.WriteLine()
			writer.Write(contents)
		ensure:
			writer.Dispose() unless writer is null
			
	def GetFirstLine(fname as string):
		try:
			f=File.OpenText(fname)
			return f.ReadLine()
		ensure:
			f.Dispose() unless f is null
			
	def print(message):
		self.Log(Level.Info, "${message}")
