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

//[AstAttributeTarget(typeof(ParameterDeclaration))]
public class RequiredAttribute(Boo.Lang.Compiler.AbstractAstAttribute):

	protected _condition as Expression
	
	public def constructor():
		pass
	
	public def constructor(condition as Expression):
		if condition is null:
			raise ArgumentNullException('condition')
		_condition = condition
	
	public override def Apply(node as Boo.Lang.Compiler.Ast.Node):
		name as string
		parent as Node
		errorMessage as string = null
		
		pd = (node as ParameterDeclaration)
		if pd is not null:
			name = pd.Name
			parent = pd.ParentNode
		else:
			prop = (node as Property)
			if (prop is not null) and (prop.Setter is not null):
				name = 'value'
				parent = prop.Setter
			else:
				InvalidNodeForAttribute('ParameterDeclaration or Property')
				return 
		
		exceptionClass as string = null
		modifier as StatementModifier = null
		if _condition is null:
			exceptionClass = 'ArgumentNullException'
			modifier = StatementModifier(StatementModifierType.If, BinaryExpression(BinaryOperatorType.ReferenceEquality, ReferenceExpression(name), NullLiteralExpression()))
		else:
			exceptionClass = 'ArgumentException'
			modifier = StatementModifier(StatementModifierType.Unless, _condition)
			errorMessage = ('Expected: ' + _condition.ToString())
		
		x = MethodInvocationExpression()
		x.Target = MemberReferenceExpression(ReferenceExpression('System'), exceptionClass)
		if errorMessage is not null:
			x.Arguments.Add(StringLiteralExpression(errorMessage))
		x.Arguments.Add(StringLiteralExpression(name))
		
		rs = RaiseStatement(x, modifier)
		rs.LexicalInfo = LexicalInfo
		
		method = (parent as Method)
		if method is not null:
			method.Body.Statements.Insert(0, rs)
		else:
			property = cast(Property, parent)
			if property.Getter is not null:
				property.Getter.Body.Statements.Insert(0, rs)
			if property.Setter is not null:
				property.Setter.Body.Statements.Insert(0, rs.CloneNode())
