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

class OnceAttribute(AbstractAstAttribute):
"""
Caches the return value of a method.

Usage
	[once]
	def foo():
		return Math.Sin(Math.PI*3)

@author Sorin Ionescu (sorin.ionescu@gmail.com)
"""
	private class ReturnValueStorageVisitor(DepthFirstVisitor):
	"""
	Replaces returns statements with binary expressions that
	store the values of the return statements.
	"""
		_returnValue as ReferenceExpression
		
		def constructor(returnValue as ReferenceExpression):
			_returnValue = returnValue
			
		override def OnReturnStatement(returnStatement as ReturnStatement):
		"""
		Replaces returns statements with binary expressions that
		store the values of the return statements.
		"""
			returnValueAssignment = BinaryExpression(
				LexicalInfo: returnStatement.LexicalInfo,
				Operator: BinaryOperatorType.Assign,
				Left: _returnValue,
				Right: returnStatement.Expression)
			
			parentNode = returnStatement.ParentNode
			parentNode.Replace(
				returnStatement,
				ExpressionStatement(returnValueAssignment))
				
	_method as Method
	_methodLock as Field
	_cached as Field
	_returnValue as Field
	
	override def Dispose():
	"""
	Dont get rid of the context because we'll need it later.
	"""
		pass
		
	override def Apply(node as Node):
	"""
	Applies the <OnceAttribute> to [node].
	
	Parameters:
		node
			The node to apply the <OnceAttribute> to.
	"""
		if not node isa Method:
			InvalidNodeForAttribute("Method")
			return
		
		_method = node
		
		CreateCachedField()
		CreateMethodLockField()
		PrepareMethodBody()		
	
		Context.Parameters.Pipeline.AfterStep += def(
			sender,
			e as CompilerStepEventArgs):
				
			return if not e.Step isa ProcessMethodBodies	
			
			# void methods dont need to be cached.
			returnType = TypeSystemServices.GetEntity(_method.ReturnType)
			return if returnType is self.TypeSystemServices.VoidType
			
			CreateReturnValueField()
			PostProcessMethod()
				
	def CreateReturnValueField():	
	"""
	Creates the field that stores the return value of the cached method.
	"""
		template = self.CodeBuilder.CreateField('field', TypeSystemServices.GetEntity(_method.ReturnType))
		template.Modifiers = TypeMemberModifiers.Private
		_returnValue = AddField(template, "___${_method.Name}_returnValue")

	def CreateCachedField():
	"""
	Creates the cached flag.
	
	Remarks:
		The flag is used to check whether the method has been cached.
	"""
		template = [|
			private field as bool
		|]
		_cached = AddField(template, "___${MethodName()}_cached")
		
	def CreateMethodLockField():
	"""
	Creates the lock field.
	
	Remarks:
		The field is used to lock on when the operatation is thread safe.
	"""
		template = [|
			private field as object = object()
		|]			
		_methodLock = AddField(template, "___${MethodName()}_lock")
		
	def MethodName():
		parentProperty = _method.ParentNode as Property
		if parentProperty is not null:
			return "${_method.Name}_${parentProperty.Name}"
		return _method.Name
		
	def AddField(template as Field, name as string):
		template.LexicalInfo = self.LexicalInfo
		template.Name = name
		template.Modifiers |= TypeMemberModifiers.Static if IsStaticMethod()
		_method.DeclaringType.Members.Add(template)
		return template
		
	def IsStaticMethod():
		return _method.IsStatic or _method.ParentNode isa Module
		
	def PrepareMethodBody():
		
		cached = ReferenceExpression(_cached.Name)
		methodLock = ReferenceExpression(_methodLock.Name)
		
		_method.Body = [|
			if not $cached:
				System.Threading.Monitor.Enter($methodLock)
				try:
					if not $cached:
						$(_method.Body)
						$cached = true
				ensure:
					System.Threading.Monitor.Exit($methodLock)
		|].ToBlock()
	
	
	def PostProcessMethod():
	"""
	Visit the node and replace return statements
	with binary expressions that store the return
	statement values.
	Add a single return statement at the end
	to return the cached value.
	"""
		_method.Accept(
				ReturnValueStorageVisitor(
					self.CodeBuilder.CreateReference(_returnValue)))
		_method.Body.Add(
			ReturnStatement(
				LexicalInfo: LexicalInfo,
				Expression: self.CodeBuilder.CreateReference(_returnValue)))
