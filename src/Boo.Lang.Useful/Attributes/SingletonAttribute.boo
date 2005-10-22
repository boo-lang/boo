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

class SingletonAttribute(AbstractAstAttribute):
"""
Implements the singleton pattern for a class.

@author Sorin Ionescu (sorin.ionescu@gmail.com)
@author Rodrigo B. de Oliveira
"""
	_singletonType as ClassDefinition
	
	override def Apply(node as Node):
		if not node isa ClassDefinition:
			InvalidNodeForAttribute("Class");
			return
		
		_singletonType = node
		
		MakeClassFinal()
		MakeConstructorsPrivate()
		CreateInstanceField()
		CreateInstanceProperty()
	
	private def MakeClassFinal():
		_singletonType.Modifiers |= TypeMemberModifiers.Final
	
	private def MakeConstructorsPrivate():
		for member in _singletonType.Members:
			ctor = member as Constructor
			if ctor is not null:
				constructorFound = true
				ctor.Modifiers |= TypeMemberModifiers.Private
		if not constructorFound:
			_singletonType.Members.Add(
				Constructor(
					LexicalInfo: self.LexicalInfo,
					Modifiers: TypeMemberModifiers.Private))
							
	private def CreateInstanceField():
		instance = Field(
					LexicalInfo: self.LexicalInfo,
					Name: "___instance",
					Modifiers: TypeMemberModifiers.Private | TypeMemberModifiers.Static,
					Type: SimpleTypeReference(_singletonType.Name))		
		_singletonType.Members.Add(instance)
		
	private def CreateInstanceProperty():		
		instanceProperty = Property(LexicalInfo: self.LexicalInfo,
								Name: "Instance",
								Modifiers: TypeMemberModifiers.Public | TypeMemberModifiers.Static)
		getter = instanceProperty.Getter = Method(LexicalInfo: self.LexicalInfo, Name: "get")
		
		ifStmt = IfStatement(LexicalInfo: self.LexicalInfo, TrueBlock: Block())
		ifStmt.Condition = BinaryExpression(
								BinaryOperatorType.ReferenceEquality,
								ReferenceExpression("___instance"),
								NullLiteralExpression())
		ifStmt.TrueBlock.Add(
			BinaryExpression(
				BinaryOperatorType.Assign,
				ReferenceExpression("___instance"),
				MethodInvocationExpression(
					ReferenceExpression(_singletonType.Name))))
		
		getter.Body.Add(ifStmt)
		getter.Body.Add(ReturnStatement(ReferenceExpression("___instance")))
			 
		_singletonType.Members.Add(instanceProperty)
