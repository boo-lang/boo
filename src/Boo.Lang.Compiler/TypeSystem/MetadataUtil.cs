#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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
using System.Reflection;
using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem
{
	/// <summary>
	/// </summary>
	public static class MetadataUtil
	{
		public static bool IsAttributeDefined(TypeMember member, IType attributeType)
		{
			foreach (Boo.Lang.Compiler.Ast.Attribute attr in member.Attributes)
			{
				IEntity entity = TypeSystemServices.GetEntity(attr);
				if (entity == attributeType)
					return true; // pre bound attribute
				IConstructor constructor = entity as IConstructor;
				if (null == constructor)
					continue;
				if (constructor.DeclaringType == attributeType)
					return true;
			}
			return false;
		}

		public static Boo.Lang.Compiler.Ast.Attribute[] GetCustomAttributes(TypeMember member, IType attributeType)
		{
			List<Boo.Lang.Compiler.Ast.Attribute> attrs = new List<Boo.Lang.Compiler.Ast.Attribute>();
			foreach (Boo.Lang.Compiler.Ast.Attribute attr in member.Attributes)
			{
				IEntity entity = TypeSystemServices.GetEntity(attr);
				if (entity == attributeType) { // pre bound attribute
					attrs.Add(attr);
					continue;
				}
				IConstructor constructor = entity as IConstructor;
				if (null == constructor)
					continue;
				if (constructor.DeclaringType == attributeType) {
					attrs.Add(attr);
					continue;
				}
			}
			return attrs.ToArray();
		}

		private static readonly MemberInfo[] NoExtensions = new MemberInfo[0];
		private static Dictionary<Type, MemberInfo[]> _clrExtensionsMembers = new Dictionary<Type, MemberInfo[]>();

		public static MemberInfo[] GetClrExtensions(Type type, string memberName)
		{
			if (!HasClrExtensions()) return NoExtensions;

			MemberInfo[] members = null;
			if (!_clrExtensionsMembers.TryGetValue(type, out members))
			{
				if (!IsAttributeDefined(type, Types.ClrExtensionAttribute))
				{
					_clrExtensionsMembers.Add(type, NoExtensions);
				}
				else
				{
					members = type.FindMembers(MemberTypes.Method, BindingFlags.Public | BindingFlags.Static, ClrExtensionFilter, memberName);
					_clrExtensionsMembers.Add(type, members);
				}
			}
			return members ?? NoExtensions;
		}

		public static bool HasClrExtensions()
		{
			return Types.ClrExtensionAttribute != null;
		}

		private static bool ClrExtensionFilter(MemberInfo member, object memberName)
		{
			return TypeUtilities.TypeName(member.Name).Equals(memberName) && IsAttributeDefined(member, Types.ClrExtensionAttribute);
		}

		public static bool IsAttributeDefined(MemberInfo member, Type attributeType)
		{
			if (Types.ClrExtensionAttribute == attributeType && null != member.DeclaringType)
			{
				MemberInfo[] members;
				if (_clrExtensionsMembers.TryGetValue(member.DeclaringType, out members))
				{
					return ((ICollection<MemberInfo>) members).Contains(member);
				}
			}
#if CHECK_ATTRIBUTES_BY_ISDEFINED
			return System.Attribute.IsDefined(member, attributeType);
#else
			// check attribute by name to account for different 
			// loaded modules (and thus different type identities)
			string attributeName = attributeType.FullName;
			System.Attribute[] attributes = System.Attribute.GetCustomAttributes(member);
			foreach (System.Attribute a in attributes)
			{
				if (a.GetType().FullName == attributeName)
					return true;
			}
			return false;
#endif
		}
	}
}

