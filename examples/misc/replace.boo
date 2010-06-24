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
Regular expression replacement tool - replaces every
occurrence of a regular expression in a group of files.

Usage:
	replace.boo <FILE GLOB> <EXPRESSION> <REPLACEMENT>
	
Examples:
	replace.boo *.cs "AssemblyVersion(.*?)" "AssemblyVersion(1.2.3.4)"
"""
import System
import System.Text.RegularExpressions
import System.IO

def Replace(folder as string, glob as string, expression as Regex, replacement as string):
	
	for fname in Directory.GetFiles(folder, glob):
		contents = File.ReadAllText(fname)
		newContents = expression.Replace(contents, replacement)
		if newContents != contents:
			print(fname)
			File.WriteAllText(fname, newContents)
			
	for subFolder in Directory.GetDirectories(folder):
		Replace(subFolder, glob, expression, replacement)

glob, expression, replacement = argv

Replace(".", glob, Regex(expression), replacement)

		
		
