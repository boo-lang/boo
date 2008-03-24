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


namespace Boo.Lang.Extensions

import System
import Boo.Lang.Compiler.Ast

class RequiredAttribute(Boo.Lang.Compiler.AbstractAstAttribute):

	_condition as Expression
	_message as Expression
	
	def constructor():
		pass
	
	def constructor(condition as Expression):
		if condition is null: raise ArgumentNullException('condition')
		_condition = condition
	
	def constructor(condition as Expression, message as Expression):
		self(condition)
		_message = message
	
	override def Apply(node as Boo.Lang.Compiler.Ast.Node):
		
		parameter = node as ParameterDeclaration
		if parameter is not null:			
			method as Method = TargetMethod(parameter)
			method.Body.Insert(0, BuildAssertion(parameter.Name))
			return
		
		property = node as Property
		CheckProperty property
		property.Setter.Body.Insert(0, BuildAssertion('value'))
		
	private def TargetMethod(parameter as ParameterDeclaration):
		method = parameter.ParentNode as Method
		return method if method is not null
		
		property = parameter.ParentNode as Property
		CheckProperty property
		return property.Setter
		
	private def CheckProperty(property as Property):
		if property is null or property.Setter is null:
			InvalidNodeForAttribute('ParameterDeclaration or Property')
		
	protected virtual def BuildAssertion(parameterName as string):
		value = ReferenceExpression(parameterName)
		if _condition is null:
			return [|
				raise System.ArgumentNullException($parameterName) if $value is null
			|]
		
		if _message is null:
			_message = StringLiteralExpression("Expected: " + _condition.ToCodeString())
			
		if _message isa StringLiteralExpression:
			return [|
				raise System.ArgumentException($_message, $parameterName) unless $_condition
			|]
		else:
			return [|
				raise System.ArgumentException(($_message).ToString(), $parameterName) unless $_condition
			|]
