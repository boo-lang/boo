#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using Boo.Ast;

namespace Boo.Lang
{
	/// <summary>
	/// Cria um acessador para um campo.
	/// </summary>
	/// <example>
	/// <pre>
	/// class Customer:
	///		[getter(FirstName)] _fname as string
	///		[getter(LastName)] _lname as string
	/// </pre>
	/// </example>
	public class GetterAttribute : AstAttribute
	{
		ReferenceExpression _propertyName;

		public GetterAttribute(ReferenceExpression propertyName)
		{
			if (null == propertyName)
			{
				throw new ArgumentNullException("propertyName");
			}
			_propertyName = propertyName;
		}

		public override void Apply(Node node)
		{
			Field f = node as Field;
			if (null == f)
			{
				throw new ApplicationException(ResourceManager.Format("InvalidNodeForAttribute", "Field"));
			}

			Property p = new Property();
			p.Name = _propertyName.Name;
			p.Type = f.Type;

			// get:
			//		return <f.Name>
			Method getter = new Method();
			getter.Name = "get";
			getter.Body.Statements.Add(
				new ReturnStatement(
					new ReferenceExpression(f.Name)
					)
				);

			p.Getter = getter;
			p.LexicalInfo = LexicalInfo;
			((TypeDefinition)f.ParentNode).Members.Add(p);
		}
	}
}
