#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
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
