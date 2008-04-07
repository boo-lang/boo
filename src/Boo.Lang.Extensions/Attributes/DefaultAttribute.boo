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
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem

//[AstAttributeTarget(typeof(ParameterDeclaration))]
public class DefaultAttribute(Boo.Lang.Compiler.AbstractAstAttribute):

	protected _value as Expression

	public def constructor(value as Expression):
		if value is null:
			raise ArgumentNullException('value')
		_value = value

	
	public override def Apply(node as Boo.Lang.Compiler.Ast.Node):
		name as string
		parent as Node
		type as IType
		
		pd = (node as ParameterDeclaration)
		if pd is not null:
			name = pd.Name
			parent = pd.ParentNode
			if pd.Type:
				type = (NameResolutionService.Resolve(pd.Type.ToString(), EntityType.Type) as IType)
			else:
				type = TypeSystemServices.ObjectType
		else:
			prop = (node as Property)
			if (prop is not null) and (prop.Setter is not null):
				name = 'value'
				parent = prop.Setter
				if prop.Type:
					type = (NameResolutionService.Resolve(prop.Type.ToString(), EntityType.Type) as IType)
				else:
					type = TypeSystemServices.ObjectType
			else:
				InvalidNodeForAttribute('ParameterDeclaration or Property')
				return 
		
		// error if parameter is a valuetype
		// TODO: check nullable (type.IsValueType true or not here?) 
		if (type is not null) and type.IsValueType:
			Errors.Add(CompilerErrorFactory.ValueTypeParameterCannotUseDefaultAttribute(parent, name))
			return 
		
		//TODO: check if default value is type-compatible with argument type?
		//TODO: handle nullable through assignIfHasValue
		assignIfNull = IfStatement(LexicalInfo)
		assignIfNull.Condition = BinaryExpression(BinaryOperatorType.ReferenceEquality, ReferenceExpression(name), NullLiteralExpression())
		assignIfNull.TrueBlock = Block(LexicalInfo)
		assignIfNull.TrueBlock.Add(BinaryExpression(BinaryOperatorType.Assign, ReferenceExpression(name), _value))
		
		method = (parent as Method)
		if method is not null:
			method.Body.Statements.Insert(0, assignIfNull)
		else:
			property = cast(Property, parent)
			if property.Setter is not null:
				property.Setter.Body.Statements.Insert(0, assignIfNull)