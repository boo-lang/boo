#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
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
