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
using Boo.Lang.Compiler.Ast.Impl;

namespace Boo.Lang.Compiler.Ast
{	
	[System.Xml.Serialization.XmlInclude(typeof(Constructor))]
	[Serializable]
	public class Method : MethodImpl
	{	
		public Method()
		{			
 		}
		
		public Method(LexicalInfo lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public Method(string name)
		{
			Name = name;
		}
		
		public bool IsOverride
		{
			get
			{
				return IsModifierSet(TypeMemberModifiers.Override);
			}
		}
		
		public bool IsVirtual
		{
			get
			{
				return IsModifierSet(TypeMemberModifiers.Virtual);
			}
		}
		
		public bool IsRuntime
		{
			get
			{
				return 
					MethodImplementationFlags.Runtime ==
						(_implementationFlags & MethodImplementationFlags.Runtime);
			}
		}
		
		override public TypeDefinition DeclaringType
		{
			get
			{
				if (null != ParentNode)
				{
					if (NodeType.Property == ParentNode.NodeType)
					{
						return (TypeDefinition)ParentNode.ParentNode;
					}					
				}
				return (TypeDefinition)ParentNode;
			}
		}
		
		override public void Accept(IAstVisitor visitor)
		{
			visitor.OnMethod(this);
		}
	}
}
