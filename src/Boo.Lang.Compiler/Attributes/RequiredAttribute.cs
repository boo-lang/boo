#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang
{
	/// <summary>
	/// Assegura que uma referncia nula no seja passada como
	/// parmetro para um mtodo.
	/// </summary>
	/// <example>
	/// <pre>
	/// def constructor([required] name as string):
	///		_name = name
	/// </pre>
	/// </example>
	//[AstAttributeTarget(typeof(ParameterDeclaration))]
	public class RequiredAttribute : Boo.Lang.Compiler.AbstractAstAttribute
	{
		public RequiredAttribute()
		{
		}

		override public void Apply(Boo.Lang.Compiler.Ast.Node node)
		{
			ParameterDeclaration pd = node as ParameterDeclaration;
			if (null == pd)
			{
				InvalidNodeForAttribute("ParameterDeclaration");
				return;
			}

			// raise ArgumentNullException("<pd.Name>") if <pd.Name> is null
			MethodInvocationExpression x = new MethodInvocationExpression();
			x.Target = new MemberReferenceExpression(
								new ReferenceExpression("System"),
								"ArgumentNullException");
			x.Arguments.Add(new StringLiteralExpression(pd.Name));
			RaiseStatement rs = new RaiseStatement(x);

			rs.Modifier = new StatementModifier(
				StatementModifierType.If,
				new BinaryExpression(BinaryOperatorType.ReferenceEquality,
					new ReferenceExpression(pd.Name),
					new NullLiteralExpression())
				);

			// associa mensagens de erro com a posio
			// do parmetro no cdigo fonte
			rs.LexicalInfo = LexicalInfo;

			Method method = pd.ParentNode as Method;
			if (null != method)
			{
				method.Body.Statements.Insert(0, rs);
			}
			else
			{
				Property property = (Property)pd.ParentNode;
				if (null != property.Getter)
				{
					property.Getter.Body.Statements.Insert(0, rs);
				}
				if (null != property.Setter)
				{
					property.Setter.Body.Statements.Insert(0, rs.CloneNode());
				}
			}
			
		}
	}
}
