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
	/// Creates a property over a field.
	/// </summary>
	public class PropertyAttribute : Boo.Lang.Compiler.AbstractAstAttribute
	{
		protected ReferenceExpression _propertyName;

		public PropertyAttribute(ReferenceExpression propertyName)
		{
			if (null == propertyName)
			{
				throw new ArgumentNullException("propertyName");
			}
			_propertyName = propertyName;
		}
		
		override public void Apply(Node node)
		{
			Field f = node as Field;
			if (null == f)
			{
				InvalidNodeForAttribute("Field");
				return;
			}			
			
			Property p = new Property();
			if (f.IsStatic)
			{
				p.Modifiers |= TypeMemberModifiers.Static;
			}
			p.Name = _propertyName.Name;
			p.Type = f.Type;
			p.Getter = CreateGetter(f);
			p.Setter = CreateSetter(f);
			p.LexicalInfo = LexicalInfo;			
			((TypeDefinition)f.ParentNode).Members.Add(p);
		}
		
		virtual protected Method CreateGetter(Field f)
		{
			// get:
			//		return <f.Name>
			Method getter = new Method();
			getter.Name = "get";
			getter.Body.Statements.Add(
				new ReturnStatement(
					new ReferenceExpression(f.Name)
					)
				);
			return getter;
		}
		
		virtual protected Method CreateSetter(Field f)
		{
			Method setter = new Method();
			setter.Name = "set";
			setter.Body.Add(
				new BinaryExpression(
					BinaryOperatorType.Assign,
					new ReferenceExpression(f.Name),
					new ReferenceExpression("value")
					)
				);
			return setter;
		}
	}
}
