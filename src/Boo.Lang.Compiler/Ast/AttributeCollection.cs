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
using System.Collections;

namespace Boo.Lang.Compiler.Ast
{
	public class AttributeCollection : Boo.Lang.Compiler.Ast.Impl.AttributeCollectionImpl
	{
		public AttributeCollection()
		{
		}
		
		public AttributeCollection(Boo.Lang.Compiler.Ast.Node parent) : base(parent)
		{
		}
		
		public bool Contains(string attributeName)
		{
			foreach (Boo.Lang.Compiler.Ast.Attribute attribute in this)
			{
				if (attributeName == attribute.Name)
				{
					return true;
				}
			}
			return false;
		}
		
		public Boo.Lang.Compiler.Ast.Attribute[] Get(string attributeName)
		{
			ArrayList attributes = new ArrayList();
			foreach (Boo.Lang.Compiler.Ast.Attribute attribute in this)
			{
				if (0 == string.Compare(attributeName, attribute.Name, true))
				{
					attributes.Add(attribute);
				}
			}
			return (Boo.Lang.Compiler.Ast.Attribute[])attributes.ToArray(typeof(Boo.Lang.Compiler.Ast.Attribute));
		}
	}
}
