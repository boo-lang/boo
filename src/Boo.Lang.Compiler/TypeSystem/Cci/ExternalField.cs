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
using System.Linq;
using Microsoft.Cci;

namespace Boo.Lang.Compiler.TypeSystem.Cci
{	
	public class ExternalField : ExternalEntity<IFieldDefinition>, IField
	{
        public ExternalField(ICciTypeSystemProvider provider, IFieldDefinition field)
            : base(provider, field)
		{	
		}
		
		public virtual IType DeclaringType
		{
			get { return _provider.Map(_memberInfo.ContainingTypeDefinition); }
		}
		
		public bool IsPublic
		{
			get { return _memberInfo.Visibility == TypeMemberVisibility.Public; }
		}
		
		public bool IsProtected
		{
			get { return _memberInfo.Visibility == TypeMemberVisibility.Family
                || _memberInfo.Visibility == TypeMemberVisibility.FamilyOrAssembly;
            }
		}

		public bool IsPrivate
		{
            get { return _memberInfo.Visibility == TypeMemberVisibility.Private; }
		}

		public bool IsInternal
		{
            get { return _memberInfo.Visibility == TypeMemberVisibility.Assembly; }
		}
		
		public bool IsStatic
		{
			get { return _memberInfo.IsStatic; }
		}
		
		public bool IsLiteral
		{
			get { return _memberInfo.CompileTimeValue.Value != null; }
		}
		
		public bool IsInitOnly
		{
			get { return _memberInfo.IsReadOnly; }
		}

        public override EntityType EntityType
		{
			get { return EntityType.Field; }
		}
		
		public virtual IType Type
		{
			get { return _provider.Map(_memberInfo.Type.ResolvedType); }
		}
		
		public object StaticValue
		{
			get { return _memberInfo.CompileTimeValue.Value; }
		}

		private static readonly ITypeReference IsVolatileType = SystemTypeMapper.GetTypeReference(
            typeof(System.Runtime.CompilerServices.IsVolatile));

		private bool? _isVolatile;

		public bool IsVolatile
		{
			get {
                if (_isVolatile == null)
				{
					_isVolatile = _memberInfo.CustomModifiers.Any(m => TypeHelper.TypesAreEquivalent(m.Modifier, IsVolatileType));
				}
				return _isVolatile.Value;
			}
		}

		public IFieldDefinition FieldInfo
		{
			get { return _memberInfo; }
		}
		
		protected override ITypeDefinition MemberType
		{
			get { return _memberInfo.Type.ResolvedType; }
		}
	}
}
