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

using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang
{
	/// <summary>
	/// Parameter default value (when parameter is not a valuetype).
	/// </summary>
	/// <example>
	/// <pre>
	/// def constructor([default("Cedric")] name as string):
	///		_name = name
	/// </pre>
	/// </example>
	//[AstAttributeTarget(typeof(ParameterDeclaration))]
	public class DefaultAttribute : Boo.Lang.Compiler.AbstractAstAttribute
	{
		protected Expression _value;
		
		public DefaultAttribute(Expression value)
		{
			if (null == value)
			{
				throw new ArgumentNullException("value");
			}
			_value = value;
		}

		override public void Apply(Boo.Lang.Compiler.Ast.Node node)
		{
			string name;
			Node parent;
			IType type;
			
			ParameterDeclaration pd = node as ParameterDeclaration;
			if (pd != null)
			{
				name = pd.Name;
				parent = pd.ParentNode;
				type = NameResolutionService.Resolve(pd.Type.ToString(), EntityType.Type) as IType;				
			}
			else
			{
				Property prop = node as Property;
				if (prop != null  && prop.Setter != null)
				{
					name = "value";
					parent = prop.Setter;
					type = NameResolutionService.Resolve(prop.Type.ToString(), EntityType.Type) as IType;
				}
				else
				{
					InvalidNodeForAttribute("ParameterDeclaration or Property");
					return;
				}
			}

			// error if parameter is a valuetype
			// TODO: check nullable (type.IsValueType true or not here?) 
			if (null != type && type.IsValueType) {
				Errors.Add(CompilerErrorFactory.ValueTypeParameterCannotUseDefaultAttribute(parent, name));
				return;
			}
			
			//TODO: check if default value is type-compatible with argument type?
			//TODO: handle nullable through assignIfHasValue
			IfStatement assignIfNull = new IfStatement(LexicalInfo);
			assignIfNull.Condition = new BinaryExpression(
										BinaryOperatorType.ReferenceEquality,
										new ReferenceExpression(name),
										new NullLiteralExpression());
			assignIfNull.TrueBlock = new Block(LexicalInfo);
			assignIfNull.TrueBlock.Add(
								new BinaryExpression(BinaryOperatorType.Assign,
			 					new ReferenceExpression(name),
								_value));

			Method method = parent as Method;
			if (null != method)
			{
				method.Body.Statements.Insert(0, assignIfNull);
			}
			else
			{
				Property property = (Property) parent;
				if (null != property.Setter)
				{
					property.Setter.Body.Statements.Insert(0, assignIfNull);
				}
			}		
		}
	}
}
