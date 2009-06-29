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
using System.Text;

namespace Boo.Lang.Compiler.Ast
{
	[System.Xml.Serialization.XmlInclude(typeof(Module))]
	[System.Xml.Serialization.XmlInclude(typeof(ClassDefinition))]
	[System.Xml.Serialization.XmlInclude(typeof(StructDefinition))]
	[System.Xml.Serialization.XmlInclude(typeof(InterfaceDefinition))]
	[System.Xml.Serialization.XmlInclude(typeof(EnumDefinition))]
	public abstract partial class TypeDefinition
	{
		protected TypeDefinition()
		{
 		}	
		
		protected TypeDefinition(LexicalInfo lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		override public string FullName
		{
			get
			{
				if (HasGenericParameters)
				{
					return string.Format(
						"{0}[of {1}]", 
						QualifiedName, 
						GenericParameters.ToCodeString());
				}
				return QualifiedName;
			}
		}

		public string QualifiedName
		{
			get
			{
				StringBuilder qualifiedName = new StringBuilder();

				TypeDefinition parentType = ParentNode as TypeDefinition;

				if (ParentNode != null && ParentNode.NodeType == NodeType.Module)
				{
					if (EnclosingNamespace != null)
					{
						qualifiedName.Append(EnclosingNamespace.Name).Append(".");
					}
				}
				else if (parentType != null)
				{
					qualifiedName.Append(parentType.FullName).Append(".");
				}

				qualifiedName.Append(Name);
				return qualifiedName.ToString();
			}
		}
		
		public bool HasMethods
		{
			get { return HasMemberOfType(NodeType.Method); }
		}

		public bool HasGenericParameters
		{
			get { return _genericParameters != null && _genericParameters.Count > 0; }
		}

		public bool HasMemberOfType(NodeType memberType)
		{
			foreach (TypeMember member in _members)
			{
				if (memberType == member.NodeType) return true;
			}
			return false;
		}


		public bool HasInstanceConstructor
		{
			get
			{
				return null != GetConstructor(0, false, null);
			}
		}

		public bool HasDeclaredInstanceConstructor
		{
			get
			{
				return null != GetConstructor(0, false, false);
			}
		}

		public bool HasStaticConstructor
		{
			get
			{
				return null != GetConstructor(0, true, null);
			}
		}

		public bool HasDeclaredStaticConstructor
		{
			get
			{
				return null != GetConstructor(0, true, false);
			}
		}

		public Constructor GetConstructor(int index)
		{
			return GetConstructor(index, null, null);
		}

		public Constructor GetStaticConstructor()
		{
			return GetConstructor(0, true, null);
		}

		protected Constructor GetConstructor(int index, bool? isStatic, bool? isSynthetic)
		{
			int current = 0;
			foreach (TypeMember member in _members)
			{
				if (NodeType.Constructor == member.NodeType)
				{
					bool match = (null == isStatic || member.IsStatic == isStatic)
						& (null == isSynthetic || member.IsSynthetic == isSynthetic);
					if (match)
					{
						if (current == index)
							return (Constructor) member;
						current++;
					}
				}
			}
			return null;
		}

	}

}
