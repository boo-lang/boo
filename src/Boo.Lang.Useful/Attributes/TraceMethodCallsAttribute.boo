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


namespace Boo.Lang.Useful.Attributes

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps

class NoTrace(AbstractAstAttribute):
	public static final Annotation = object()
	
	override def Apply(node as Node):
		node.Annotate(NoTrace.Annotation)

class TraceMethodCallsAttribute(AbstractAstAttribute):
"""
	Visits every method adding a trace statement at both its very
	beginning and end.
"""	

	[property(TraceMethod)]
	_traceMethod as ReferenceExpression
	
	def constructor():
		self([| System.Console.WriteLine |]);
		
	def constructor(traceMethod as ReferenceExpression):
		_traceMethod = traceMethod
		
	override def Apply(node as Node):
		node.Accept(TraceVisitor(_traceMethod))
		
	class IsGeneratorVisitor(DepthFirstVisitor):
		
		[getter(IsGenerator)]
		_isGenerator = false
		
		override def OnBlockExpression(node as BlockExpression):
			pass
		
		override def OnYieldStatement(node as YieldStatement):
			_isGenerator = true
		
	class TraceVisitor(DepthFirstVisitor):
		
		_traceMethod as ReferenceExpression
		
		def constructor(traceMethod as ReferenceExpression):
			_traceMethod = traceMethod
			
		def GetFullName(method as Method):
			if IsModuleClass(method.DeclaringType):
				module as Module = method.EnclosingModule
				if module.Namespace is null: return method.Name
				return "${module.Namespace.Name}.{method.Name}"
			
			prop = method.ParentNode as Property
			if prop is not null:
				return prop.FullName + "." + method.Name
				
			return method.FullName
			
				
		def IsModuleClass(type as TypeDefinition):
			return Boo.Lang.Compiler.Steps.IntroduceModuleClasses.IsModuleClass(type)
		
		override def LeaveMethod(method as Method):
			
			//print method, method.ContainsAnnotation(NoTrace.Annotation)
			if method.ContainsAnnotation(NoTrace.Annotation):
				return
			
			if not HasBody(method):
				return
			
			fullName = GetFullName(method)
			
			method.Body.Insert(0, TraceCall("TRACE: Entering ${fullName}"))
			if IsGenerator(method):
				return
				
			method.Body = [|
				try:
					$(method.Body)
				ensure:
					$(TraceCall("TRACE: Leaving ${fullName}"))
			|].ToBlock()
			
		def IsGenerator(method as Method):
			visitor = IsGeneratorVisitor()
			method.Body.Accept(visitor)
			return visitor.IsGenerator
			
		def HasBody(method as Method):
			if method.DeclaringType isa InterfaceDefinition: return false
			if method.Body is null or len(method.Body.Statements) == 0: return false
			return true
			
		def TraceCall(msg as string):
			return [| $_traceMethod($msg) |]
			
