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

import System.Threading
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem

class OnceAttribute(AbstractAstAttribute):
"""
Caches the return value of a method.

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
				
	_codeBuilder as BooCodeBuilder
	_compilerContext as CompilerContext
	_typeSystemServices as TypeSystemServices
	_method as Method
	_fieldModifiers as TypeMemberModifiers
	_methodLock as Field
	_cached as Field
	_returnValue as Field
	_constructors = []
		
	override def Apply(node as Node):
	"""
	Applies the <OnceAttribute> to [node].
	
	Parameters:
		node
			The node to apply the <OnceAttribute> to.
	"""
		assert node isa Method
		
		_method = node
		
		# The follwing must be cached since they will be
		# lost once the method returns.
		_codeBuilder = CodeBuilder
		_compilerContext = Context
		_typeSystemServices = TypeSystemServices
		
		_fieldModifiers = TypeMemberModifiers.Private
		_fieldModifiers |= TypeMemberModifiers.Static if _method.IsStatic
		
		Context.Parameters.Pipeline.AfterStep += def(
			sender,
			e as CompilerStepEventArgs):
				
			if e.Step isa ProcessMethodBodies:	
				# Void methods cannot be cached.
				assert _method.ReturnType.Entity is not \
					_compilerContext.TypeSystemServices.VoidType
				
				CreateReturnValueField()
				CreateCachedField()
				CreateMethodLockField()
				GetConstructors()
				
				if len(_constructors) == 0:
					CreateConstructor()
				
				CreateMethodLockFieldInitialization()
				
				# Visit the node and replace return statements
				# with binary expressions that store the return
				# statement values.
				returnVisitor = ReturnValueStorageVisitor(
					_codeBuilder.CreateReference(_returnValue))
				
				returnVisitor.Visit(_method)
				
				MakeMethodCacheable()
				
	def CreateReturnValueField():	
	"""
	Creates the field that stores the return value of the cached method.
	"""
		_returnValue = _codeBuilder.CreateField(
			"___${_method.Name}_returnValue",
			_method.ReturnType.Entity)
		
		_returnValue.Modifiers = _fieldModifiers
	
		_method.DeclaringType.Members.Add(_returnValue)


	def CreateCachedField():
	"""
	Creates the cached flag.
	
	Remarks:
		The flag is used to check whether the method has been cached.
	"""
		_cached = _codeBuilder.CreateField(
			"___${_method.Name}_cached",
			_typeSystemServices.Map(bool))
		
		_cached.Modifiers = _fieldModifiers
		
		_method.DeclaringType.Members.Add(_cached)
		
	def CreateMethodLockField():
	"""
	Creates the lock field.
	
	Remarks:
		The field is used to lock on when the operatation is thread safe.
	"""
		_methodLock = _codeBuilder.CreateField(
			"___${_method.Name}_lock",
			_typeSystemServices.Map(object))
		
		_methodLock.Modifiers = _fieldModifiers
		
		objectConstructors = _typeSystemServices.Map(object).GetConstructors()
		objectConstructor = objectConstructors[0]
		_methodLock.Initializer = _codeBuilder.CreateConstructorInvocation(
			objectConstructor)
		
		_method.DeclaringType.Members.Add(_methodLock)
	
	def MakeMethodCacheable():
	"""
	Modifies the method body to make it cacheable.
	"""
		methodBody = Block(LexicalInfo)
		
		# Create the double checked lock pattern.
		outerIfNotCached = IfStatement(
			LexicalInfo: LexicalInfo,
			Condition: BinaryExpression(
				LexicalInfo,
				BinaryOperatorType.Inequality,
				_codeBuilder.CreateReference(_cached),
				_codeBuilder.CreateBoolLiteral(true)),
			TrueBlock: Block())	
			
		innerIfNotCached = outerIfNotCached.CloneNode()
		innerIfNotCached.TrueBlock.Add(_method.Body)
		innerIfNotCached.TrueBlock.Add(
			BinaryExpression(
				LexicalInfo,
				BinaryOperatorType.Assign,
				_codeBuilder.CreateReference(_cached),
				_codeBuilder.CreateBoolLiteral(true)))
			
		monitorEnterInvocation = _codeBuilder.CreateMethodInvocation(
			_typeSystemServices.Map(typeof(Monitor).GetMethod('Enter')),
			_codeBuilder.CreateReference(_methodLock))
		
		monitorExitInvocation = _codeBuilder.CreateMethodInvocation(
			_typeSystemServices.Map(typeof(Monitor).GetMethod('Exit')),
			_codeBuilder.CreateReference(_methodLock))
		
		protectedBlock = Block(LexicalInfo)
		protectedBlock.Add(innerIfNotCached)
		
		ensureBlock = Block(LexicalInfo)
		ensureBlock.Add(monitorExitInvocation)
		
		outerIfNotCached.TrueBlock.Add(monitorEnterInvocation)			
		outerIfNotCached.TrueBlock.Add(
			TryStatement(
				LexicalInfo: LexicalInfo,
				ProtectedBlock: protectedBlock,
				EnsureBlock: ensureBlock))
		
		methodBody.Add(outerIfNotCached)
		methodBody.Add(
			ReturnStatement(
				LexicalInfo: LexicalInfo,
				Expression: _codeBuilder.CreateReference(_returnValue)))
				
		_method.Body = methodBody
		
	private def GetConstructors():
	"""
	Gets the constructors.
	
	Returns:
		The constructors of the type.
	"""
		for member in _method.DeclaringType.Members:
			ctor = member as Constructor
			if ctor is not null:
				if (ctor.IsStatic and _methodLock.IsStatic) or \
					(not ctor.IsStatic and not _methodLock.IsStatic):
						_constructors.Add(ctor)
	
	private def CreateConstructor():
	"""
	Creates the default constructor.
	"""
		ctorModifiers = TypeMemberModifiers.Public
		
		if _methodLock.IsStatic:
			ctorModifiers |= TypeMemberModifiers.Static
		
		ctor = _codeBuilder.CreateConstructor(ctorModifiers)
		
		_constructors.Add(ctor)
		_method.DeclaringType.Members.Add(ctor)
	
	private def CreateMethodLockFieldInitialization():
	"""
	Initializes the field.
	"""
		for ctor as Constructor in _constructors:
			ctor.Body.Insert(
				0,
				BinaryExpression(
					LexicalInfo,
					BinaryOperatorType.Assign,
					_codeBuilder.CreateReference(_methodLock),
					_methodLock.Initializer))
					
		_methodLock.Initializer = null