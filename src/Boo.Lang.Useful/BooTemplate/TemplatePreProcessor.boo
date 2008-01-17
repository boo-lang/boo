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


namespace Boo.Lang.Useful.BooTemplate

import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Steps

class TemplatePreProcessor(AbstractCompilerStep):
	override def Run():
		new = []
		for input in self.Parameters.Input:
			using reader=input.Open():
				code = booify(reader.ReadToEnd())
				new.Add(StringInput(input.Name, code))
		self.Parameters.Input.Clear()
		for input in new:
			self.Parameters.Input.Add(input)

def booify(code as string):
	buffer = System.IO.StringWriter()
	output = def(code as string):
		return if len(code) == 0
		buffer.Write('Output.Write("""')
		buffer.Write(code)
		buffer.WriteLine('""")')

	lastIndex = 0
	index = code.IndexOf("<%")
	while index > -1:
		output(code[lastIndex:index])
		lastIndex = code.IndexOf("%>", index + 2)
		raise 'expected %>' if lastIndex < 0
		buffer.WriteLine(code[index+2:lastIndex])
		lastIndex += 2
		index = code.IndexOf("<%", lastIndex)
		
	output(code[lastIndex:])
	return buffer.ToString()

