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
	[System.Xml.Serialization.XmlInclude(typeof(TypeDefinition))]
	[System.Xml.Serialization.XmlInclude(typeof(EnumMember))]
	[System.Xml.Serialization.XmlInclude(typeof(Field))]
	[System.Xml.Serialization.XmlInclude(typeof(Property))]
	[System.Xml.Serialization.XmlInclude(typeof(Method))]
	[Serializable]
	public abstract class TypeMember : TypeMemberImpl
	{		
		protected TypeMember()
		{
 		}
		
		protected TypeMember(TypeMemberModifiers modifiers, string name) : base(modifiers, name)
		{
		}		
		
		protected TypeMember(LexicalInfo lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public virtual TypeDefinition DeclaringType
		{
			get
			{
				return (TypeDefinition)ParentNode;
			}
		}
		
		public virtual string FullName
		{
			get
			{
				if (null != ParentNode)
				{
					return DeclaringType.FullName + "." + Name;
				}
				return Name;
			}
		}
		
		public virtual NamespaceDeclaration EnclosingNamespace
		{
			get
			{
				Node parent = _parent;
				while (parent != null)
				{
					Module module = parent as Module;
					if (null != module)
					{
						return module.Namespace;
					}
					parent = parent.ParentNode;
				}
				return null;
			}
		}
		
		public bool IsVisibilitySet
		{
			get
			{
				return IsPublic | IsInternal | IsPrivate | IsProtected;
			}
		}
		
		public bool IsAbstract
		{
			get
			{
				return IsModifierSet(TypeMemberModifiers.Abstract);
			}			
		}
		
		public bool IsStatic
		{
			get
			{
				return IsModifierSet(TypeMemberModifiers.Static);
			}
		}
		
		public bool IsPublic
		{
			get
			{
				return IsModifierSet(TypeMemberModifiers.Public);
			}
		}
		
		public bool IsInternal
		{
			get
			{
				return IsModifierSet(TypeMemberModifiers.Internal);
			}
		}
		
		public bool IsProtected
		{
			get
			{
				return IsModifierSet(TypeMemberModifiers.Protected);
			}
		}
		
		public bool IsPrivate
		{
			get
			{
				return IsModifierSet(TypeMemberModifiers.Private);
			}
		}
		
		public bool IsFinal
		{
			get
			{
				return IsModifierSet(TypeMemberModifiers.Final);
			}
		}
		
		public bool IsTransient
		{
			get
			{
				return IsModifierSet(TypeMemberModifiers.Transient);
			}
		}
		
		public bool IsModifierSet(TypeMemberModifiers modifiers)
		{
			return modifiers == (_modifiers & modifiers);
		}
		
		override public string ToString()
		{
			return FullName;
		}
	}
}
