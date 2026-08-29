#region license
// Copyright (c) 2005, Sorin Ionescu (sorin.ionescu@gmail.com)
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
//     * Neither the name of Sorin Ionescu nor the names of its
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

namespace Boo.Examples.Attributes

import System;
import System.Runtime.Remoting.Messaging
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps

class AsyncAttribute(AbstractAstAttribute):
"""
Adds asynchronous helpers Begin<Method>/End<Method>
for a method.
"""	
	_method          as Method
	_accessModifiers as TypeMemberModifiers
	_disposed		 as ReferenceExpression
	
	def constructor():
		pass
		
	def constructor(disposed as ReferenceExpression):
		_disposed = disposed
	
	override def Apply(node as Node):	
		assert node isa Method
		
		_method = node
		
		if _method.IsProtected and _method.IsInternal:
			_accessModifiers = TypeMemberModifiers.Protected
			_accessModifiers = _accessModifiers | TypeMemberModifiers.Internal
			
		elif _method.IsPublic:
			_accessModifiers = TypeMemberModifiers.Public
			
		elif _method.IsProtected:
			_accessModifiers = TypeMemberModifiers.Protected
			
		elif _method.IsInternal:
			_accessModifiers = TypeMemberModifiers.Internal
			
		elif _method.IsPrivate:
			_accessModifiers = TypeMemberModifiers.Private
		
		EmitBeginMethod()
		EmitEndMethod()
	
	private def EmitBeginMethod():
		beginMethod = Method(
						self.LexicalInfo,
						Name: "Begin" + _method.Name,
						Modifiers: _accessModifiers,
						ReturnType: CodeBuilder.CreateTypeReference(typeof(IAsyncResult)))
		beginMethod.Parameters.ExtendWithClones(_method.Parameters)			
		beginMethod.Parameters.Add(
				ParameterDeclaration(
					"callback",
					CodeBuilder.CreateTypeReference(typeof(AsyncCallback))))
		beginMethod.Parameters.Add(
				ParameterDeclaration(
					"state",
					CodeBuilder.CreateTypeReference(typeof(object))))
		
		asyncInvocation = MethodInvocationExpression(
							Target: MemberReferenceExpression(
										ReferenceExpression(_method.Name),
										"BeginInvoke"))
		for parameter in beginMethod.Parameters:
			asyncInvocation.Arguments.Add(
					ReferenceExpression(parameter.Name))
		
		EmitDisposedObjectCheck(beginMethod) if _disposed is not null
		beginMethod.Body.Add(ReturnStatement(asyncInvocation))			
		_method.DeclaringType.Members.Add(beginMethod)
		
	private def EmitEndMethod():
		endMethod = Method(
						self.LexicalInfo,
						Name: "End" + _method.Name,
						Modifiers: _accessModifiers)		
		endMethod.Parameters.Add(
			ParameterDeclaration(
				"result",
				CodeBuilder.CreateTypeReference(typeof(IAsyncResult))))	
		
		asyncInvocation = MethodInvocationExpression(
							Target: MemberReferenceExpression(
										ReferenceExpression(_method.Name),
										"EndInvoke"))			
		asyncInvocation.Arguments.Add(ReferenceExpression("result"))
		
		EmitDisposedObjectCheck(endMethod) if _disposed is not null
		endMethod.Body.Add(ReturnStatement(asyncInvocation))
		
		_method.DeclaringType.Members.Add(endMethod)
		
		# cache the voidType reference because we are going
		# to lose the context after this method returns
		# (see AbstractCompilerComponent.Dispose)
		voidType = Context.TypeSystemServices.VoidType
		Context.Parameters.Pipeline.AfterStep += def (sender, e as CompilerStepEventArgs):
			if e.Step isa ProcessMethodBodies:
				if _method.ReturnType.Entity is voidType:
					returnStatement = endMethod.Body.Statements[-1] as ReturnStatement
					endMethod.Body.Statements.Replace(
							returnStatement,
							ExpressionStatement(returnStatement.Expression))
							
	private def EmitDisposedObjectCheck(method as Method):
		exceptionCreation = MethodInvocationExpression(
								Target: MemberReferenceExpression(
											ReferenceExpression("System"), "ObjectDisposedException"))								
		exceptionCreation.Arguments.Add(StringLiteralExpression(_method.DeclaringType.Name))
		# TODO: Access Boo resources to get the exception message.
		exceptionCreation.Arguments.Add(StringLiteralExpression("<Resources.ObjectDisposedExceptionMessage>"))
		
		trueBlock = Block()
		trueBlock.Add(RaiseStatement(exceptionCreation))
		method.Body.Add(IfStatement(_disposed.CloneNode(), trueBlock, null)) 
		