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
Like booi except it only spits out the XML representation of the AST.

booi.exe examples\ast-to-xml.boo file1.boo file2.boo

To show the AST after the first parsing step only use the -parse option:
booi.exe examples\ast-to-xml.boo -parse file1.boo file2.boo

If you want to save the output to a file:
booi.exe examples\ast-to-xml.boo file1.boo file2.boo > output.xml
"""

import System
import System.IO
import System.Xml.Serialization from System.Xml
import Boo.Lang.Compiler from Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines

def PrintAST([required]result as CompilerContext, [required]out as TextWriter):
	astobject = result.CompileUnit
	try:
		s = XmlSerializer( astobject.GetType() )
	except e:
		print e.Message
		return
	try:
		s.Serialize( out, astobject )
	except e:
		print "\n", e.ToString()


compiler = BooCompiler()
files as (string)

if argv[0] == "-parse":
	compiler.Parameters.Pipeline = Parse()
	files = argv[1:]
else:
	compiler.Parameters.Pipeline = Compile()
	files = argv[:]

if len(files) == 0:
	print "please specify at least one boo file as a parameter"
	return
	
for filename in files:
	compiler.Parameters.Input.Add(FileInput(filename))

try:
	result = compiler.Run()
	if len(result.Errors) > 0:
		print "There were errors compiling the boo file(s)"
	else:
		PrintAST(result, Console.Out)
except e:
	print e.GetType(), ":", e.Message

