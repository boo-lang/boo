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


import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem

class EverybodyLovesDucks(ProcessMethodBodiesWithDuckTyping):
	
	_getInRuntime as IMethod
	
	override def OnReferenceExpression(node as ReferenceExpression):
		entity = self.NameResolutionService.Resolve(node.Name)
		if entity is not null:
			super(node)
		else:
			mie = CodeBuilder.CreateMethodInvocation(
					CodeBuilder.CreateSelfReference(self._currentMethod.DeclaringType),
					_getInRuntime)
			mie.Arguments.Add(CodeBuilder.CreateStringLiteral(node.Name))
			node.ParentNode.Replace(node, mie)
			
	override def InitializeMemberCache():
		super()
		_getInRuntime = TypeSystemServices.Map(typeof(BaseTemplate).GetMethod("GetInRuntime"))

class ProcessTemplate(AbstractVisitorCompilerStep):
	override def Run():
		Visit(self.CompileUnit)
		
	override def OnModule(node as Module):
		template = ClassDefinition(Name: "Template")
		template.BaseTypes.Add(CodeBuilder.CreateTypeReference(BaseTemplate))
		template.Members.Add(
			Method(Name: "Run", Body: node.Globals))
			
		node.Members.Add(template)
		node.Globals = Block()
		
class BaseTemplate:
	def GetInRuntime(name as string):
		return "name is: ${name}"
		
	abstract def Run():
		pass
		
template = """
print name
"""

pipeline = Pipelines.CompileToMemory()
pipeline.InsertAfter(InitializeTypeSystemServices, ProcessTemplate())
pipeline.Replace(ProcessMethodBodiesWithDuckTyping, EverybodyLovesDucks())

compiler = BooCompiler()
compiler.Parameters.Input.Add(IO.StringInput("template.boo", template))
compiler.Parameters.Pipeline = pipeline
compiler.Parameters.OutputType = CompilerOutputType.Library


try:
	result = compiler.Run()
	print result.Errors.ToString(true) if len(result.Errors)
	
	print "the generated code looks like this:"
	print
	print result.CompileUnit
	
	assembly = result.GeneratedAssembly	
	assert assembly is not null
	
	templateType = assembly.GetType("Template")
	templateInstance as BaseTemplate = templateType()
	templateInstance.Run()
	
except x:
	print x
