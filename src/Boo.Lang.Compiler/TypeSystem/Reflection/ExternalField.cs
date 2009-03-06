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
using Boo.Lang.Compiler.TypeSystem.Reflection;

namespace Boo.Lang.Compiler.TypeSystem
{	
	public class ExternalField : ExternalEntity<System.Reflection.FieldInfo>, IField
	{
		public ExternalField(IReflectionTypeSystemProvider provider, System.Reflection.FieldInfo field) : base(provider, field)
		{	
		}
		
		public virtual IType DeclaringType
		{
			get { return _provider.Map(_memberInfo.DeclaringType); }
		}
		
		public bool IsPublic
		{
			get { return _memberInfo.IsPublic; }
		}
		
		public bool IsProtected
		{
			get { return _memberInfo.IsFamily || _memberInfo.IsFamilyOrAssembly; }
		}

		public bool IsPrivate
		{
			get { return _memberInfo.IsPrivate; }
		}

		public bool IsInternal
		{
			get { return _memberInfo.IsAssembly; }
		}
		
		public bool IsStatic
		{
			get { return _memberInfo.IsStatic; }
		}
		
		public bool IsLiteral
		{
			get { return _memberInfo.IsLiteral; }
		}
		
		public bool IsInitOnly
		{
			get { return _memberInfo.IsInitOnly; }
		}
		
		override public EntityType EntityType
		{
			get { return EntityType.Field; }
		}
		
		public virtual IType Type
		{
			get { return _provider.Map(_memberInfo.FieldType); }
		}
		
		public object StaticValue
		{
			get { return _memberInfo.GetValue(null); }
		}

		static readonly Type IsVolatileType = typeof(System.Runtime.CompilerServices.IsVolatile);
		bool? _isVolatile;

		public bool IsVolatile
		{
			get {
				if (null == _isVolatile)
				{
					Type[] mods = _memberInfo.GetRequiredCustomModifiers();
					_isVolatile = false;
					foreach (Type mod in mods)
					{
						if (mod == IsVolatileType)
						{
							_isVolatile = true;
							break;
						}
					}
				}
				return _isVolatile.Value;
			}
		}

		public System.Reflection.FieldInfo FieldInfo
		{
			get { return _memberInfo; }
		}
		
		protected override Type MemberType
		{
			get { return _memberInfo.FieldType; }
		}
	}
}
