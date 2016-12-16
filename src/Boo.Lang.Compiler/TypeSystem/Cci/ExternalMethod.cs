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

using System.Linq;
using Boo.Lang.Environments;
using Microsoft.Cci;

namespace Boo.Lang.Compiler.TypeSystem.Cci
{
	using System;

	public class ExternalMethod : ExternalEntity<IMethodDefinition>, IMethod, IEquatable<ExternalMethod>
	{
		protected IParameter[] _parameters;
		private bool? _acceptVarArgs;
		private bool? _isPInvoke;
		private bool? _isMeta;

        internal ExternalMethod(ICciTypeSystemProvider provider, IMethodDefinition mi)
            : base(provider, mi)
		{
		}

	    private static readonly ITypeReference _metaAttribute = SystemTypeMapper.GetTypeReference(typeof(MetaAttribute));

		public bool IsMeta
		{
			get
			{
				if (_isMeta == null)
					_isMeta = IsStatic && MetadataUtil.IsAttributeDefined(_memberInfo, _metaAttribute);
				return _isMeta.Value;
			}
		}

	    private static readonly ITypeReference _importAttribute = SystemTypeMapper.GetTypeReference(Types.DllImportAttribute);

		public bool IsPInvoke
		{
			get
			{
				if (_isPInvoke == null)
					_isPInvoke = IsStatic && MetadataUtil.IsAttributeDefined(_memberInfo, _importAttribute);
				return _isPInvoke.Value;
			}
		}
		
		public virtual IType DeclaringType
		{
			get { return _provider.Map(_memberInfo.ContainingTypeDefinition); }
		}
		
		public bool IsStatic
		{
			get { return _memberInfo.IsStatic; }
		}
		
		public bool IsPublic
		{
			get { return _memberInfo.Visibility == TypeMemberVisibility.Public; }
		}
		
		public bool IsProtected
		{
            get { return _memberInfo.Visibility == TypeMemberVisibility.Family 
                || _memberInfo.Visibility == TypeMemberVisibility.FamilyOrAssembly; }
		}

		public bool IsPrivate
		{
            get { return _memberInfo.Visibility == TypeMemberVisibility.Private; }
		}
		
		public bool IsAbstract
		{
			get { return _memberInfo.IsAbstract; }
		}

		public bool IsInternal
		{
            get { return _memberInfo.Visibility == TypeMemberVisibility.Assembly; }
		}
		
		public bool IsVirtual
		{
			get { return _memberInfo.IsVirtual; }
		}

		public bool IsFinal
		{
			get { return _memberInfo.IsSealed;  }
		}
		
		public bool IsSpecialName
		{
			get { return _memberInfo.IsSpecialName; }
		}
		
		public bool AcceptVarArgs
		{
			get
			{
				if (_acceptVarArgs == null)
				{
					var parameters = _memberInfo.Parameters.ToArray();
                    _acceptVarArgs = parameters.Length > 0 && parameters[parameters.Length-1].IsParameterArray;
				}
				return _acceptVarArgs.Value;
			}
		}

        public override EntityType EntityType
		{
			get { return EntityType.Method; }
		}
		
		public ICallableType CallableType
		{
			get { return My<TypeSystemServices>.Instance.GetCallableType(this); }
		}

		public IType Type
		{
			get { return CallableType; }
		}
		
		public virtual IParameter[] GetParameters()
		{
			if (null != _parameters) return _parameters;
			return _parameters = _provider.Map(_memberInfo.Parameters.ToArray());
		}

		public virtual IType ReturnType
		{
			get
			{
                return _provider.Map(_memberInfo.Type.ResolvedType);
			}
		}

		public IMethodDefinition MethodInfo
		{
			get { return _memberInfo; }
		}

        public override bool Equals(object other)
		{
			if (null == other) return false;
			if (this == other) return true;

			ExternalMethod method = other as ExternalMethod;
			return Equals(method);
		}

		public bool Equals(ExternalMethod other)
		{
			if (other == null) return false;
			if (this == other) return true;

			return _memberInfo.InternedKey == other._memberInfo.InternedKey;
		}

        public override int GetHashCode()
		{
			return _memberInfo.GetHashCode();
		}

        public override string ToString()
		{
			return _memberInfo.ToString();
		}
		
		ExternalGenericMethodInfo _genericMethodDefinitionInfo;		
		public IGenericMethodInfo GenericInfo
		{
			get
			{
				if (!MethodInfo.IsGenericMethodDefinition)
					return null;

				return _genericMethodDefinitionInfo ??
				       (_genericMethodDefinitionInfo = new ExternalGenericMethodInfo(_provider, this));
			}
		}

		ExternalConstructedMethodInfo _genericMethodInfo;
		public virtual IConstructedMethodInfo ConstructedInfo
		{
			get
			{
				if (!MethodInfo.IsGeneric)
					return null;

				return _genericMethodInfo ?? (_genericMethodInfo = new ExternalConstructedMethodInfo(_provider, this));
			}
		}

		protected override ITypeDefinition MemberType
		{
			get
			{
				return _memberInfo.Type.ResolvedType;
			}
		}
	}
}
