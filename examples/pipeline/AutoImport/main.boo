#region license
// Copyright (c) 2003, 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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


import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps
import System.IO

class AutoImport(AbstractVisitorCompilerStep):

	override def OnImport(node as Import):

		references = self.Parameters.References
		errors = self.Errors
		
		for reference in references:
			simpleName = @/, /.Split(reference.FullName)[0]
			return if simpleName == node.Namespace			
		
		result = compile("${node.Namespace}.boo", CompilerOutputType.Library)
		if len(result.Errors):
			for e in result.Errors:
				errors.Add(e)
		else: 			
			references.Add(result.GeneratedAssembly)

	override def Run():
		Visit(CompileUnit)

def compile(fname as string, outputType as CompilerOutputType):
	pipeline = CompileToMemory()
	pipeline.Insert(1, AutoImport())
	
	print("compiling ${fname}...")
	compiler = BooCompiler()
	compiler.Parameters.OutputType = outputType
	compiler.Parameters.Input.Add(FileInput(fname))
	compiler.Parameters.Pipeline = pipeline		
	result = compiler.Run()
	print("done.")
	return result

result = compile("client.boo", CompilerOutputType.ConsoleApplication)
if len(result.Errors):
	print(result.Errors.ToString(true))
else:
	result.GeneratedAssemblyEntryPoint.Invoke(null, (null,))
		
	
	 
