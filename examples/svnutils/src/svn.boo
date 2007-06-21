#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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
svn utility functions
"""
import System
import System.IO

class ResourceStatus:
	static def parse(line as string):
		parts = /\s+/.Split(line.Trim(), 2)
		return ResourceStatus(code: parts[0], resource: parts[1])
		
	public code as string
	public resource as string
	
	override def ToString():
		return "${code}\t${resource}"

def svn_status(resource as string):
	return parse_status(shell("svn", "status ${resource}"))
	
def parse_status(status as string):
	for line in lines(status):
		yield ResourceStatus.parse(line)

def svn_pg(resource as string, propertyName as string):
	return shell("svn", "pg ${propertyName} ${resource}")
	
def svn_ps(resource as string, propertyName as string, propertyValue as string):
	tempFile = ".svn_ignore"
	File.WriteAllText(tempFile, propertyValue)
	try:
		return shell("svn", "ps ${propertyName} --file \"${tempFile}\" ${resource}")
	ensure:
		File.Delete(tempFile)
		
def svn_locals(resource as string):
	return (
		status.resource
		for status in svn_status(resource)
		if status.code == "?")
		
def svn_ignore(resource as string, whatToIgnore as string):
	current = svn_pg(resource, "svn:ignore")
	print svn_ps(resource, "svn:ignore", current.Trim() + "\n" + whatToIgnore)
		
def lines(s as string):
	return line.Trim() for line in /(\r?\n)+/.Split(s) if len(line.Trim())
	
def confirm(message as string):
	return "y" == prompt("${message} (y/n): ")

