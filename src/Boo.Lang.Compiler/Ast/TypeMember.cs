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
