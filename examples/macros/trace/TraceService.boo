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
Description:
	TraceService class is the compiler service used by `trace` macro.
	Read `trace.boo` for more information.
"""

import System
import System.IO
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast


class TraceService (IDisposable):
	context as CompilerContext
	locations = List[of string]()

	def constructor(context as CompilerContext):
		.context = context

	def AddLocation(lexicalInfo as LexicalInfo, method as Method):
		locations.Add("${lexicalInfo.ToString()} : ${method.ToString()}")

	def Dispose():
		if len(context.Errors):
			return #do not write file if there was an error during compilation

		output = context.Parameters.OutputAssembly
		return unless output #do not write file if there no output assembly (eg. compilation in memory)

		print "NOTICE: writing trace locations to file `${output}.traces`"
		using writer = StreamWriter(output+".traces"):
			for location in locations:
				writer.WriteLine(location)

