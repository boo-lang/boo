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

using Boo.Lang.Compiler.TypeSystem.Reflection;

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Reflection;

	public class ExternalMethod : ExternalEntity<MethodBase>, IMethod, IEquatable<ExternalMethod>
	{
		protected IParameter[] _parameters;

		private bool? _acceptVarArgs;

		private bool? _isBooExtension;
		private bool? _isClrExtension;

		private bool? _isPInvoke;
		private bool? _isMeta;


		internal ExternalMethod(IReflectionTypeSystemProvider provider, MethodBase mi) : base(provider, mi)
		{
		}

		public bool IsMeta
		{
			get
			{
				if (null == _isMeta)
				{
					_isMeta = IsStatic && MetadataUtil.IsAttributeDefined(_memberInfo, typeof(Boo.Lang.MetaAttribute));
				}
				return _isMeta.Value;
			}
		}

		public bool IsExtension
		{
			get
			{
				return IsBooExtension || IsClrExtension;
			}
		}

		public bool IsBooExtension
		{
			get
			{
				if (null == _isBooExtension)
				{
					_isBooExtension = MetadataUtil.IsAttributeDefined(_memberInfo, Types.BooExtensionAttribute);
				}
				return _isBooExtension.Value;
			}
		}

		public bool IsClrExtension
		{
			get
			{
				if (null == _isClrExtension)
				{
					_isClrExtension = MetadataUtil.HasClrExtensions()
							&& IsStatic
							&& MetadataUtil.IsAttributeDefined(_memberInfo, Types.ClrExtensionAttribute);
				}
				return _isClrExtension.Value;
			}
		}

		public bool IsPInvoke
		{
			get
			{
				if (null == _isPInvoke)
				{
					_isPInvoke = IsStatic && MetadataUtil.IsAttributeDefined(_memberInfo,  Types.DllImportAttribute);
				}
				return _isPInvoke.Value;
			}
		}
		
		public virtual IType DeclaringType
		{
			get
			{
				return _provider.Map(_memberInfo.DeclaringType);
			}
		}
		
		public bool IsStatic
		{
			get
			{
				return _memberInfo.IsStatic;
			}
		}
		
		public bool IsPublic
		{
			get
			{
				return _memberInfo.IsPublic;
			}
		}
		
		public bool IsProtected
		{
			get
			{
				return _memberInfo.IsFamily || _memberInfo.IsFamilyOrAssembly;
			}
		}

		public bool IsPrivate
		{
			get
			{
				return _memberInfo.IsPrivate;
			}
		}
		
		public bool IsAbstract
		{
			get
			{
				return _memberInfo.IsAbstract;
			}
		}

		public bool IsInternal
		{
			get
			{
				return _memberInfo.IsAssembly;
			}
		}
		
		public bool IsVirtual
		{
			get
			{
				return _memberInfo.IsVirtual;
			}
		}
		
		public bool IsSpecialName
		{
			get
			{
				return _memberInfo.IsSpecialName;
			}
		}
		
		public bool AcceptVarArgs
		{
			get
			{
				if (null == _acceptVarArgs)
				{
					ParameterInfo[] parameters = _memberInfo.GetParameters();
					_acceptVarArgs =
						parameters.Length > 0 && IsParamArray(parameters[parameters.Length-1]);
				}
				return _acceptVarArgs.Value;
			}
		}

		private bool IsParamArray(ParameterInfo parameter)
		{
			/* Hack to fix problem with mono-1.1.8.* and older */
			return parameter.ParameterType.IsArray
				&& (
					Attribute.IsDefined(parameter, Types.ParamArrayAttribute)
					|| parameter.GetCustomAttributes(Types.ParamArrayAttribute, false).Length > 0);
		}

		
		override public EntityType EntityType
		{
			get
			{
				return EntityType.Method;
			}
		}
		
		public ICallableType CallableType
		{
			get
			{
				return My<TypeSystemServices>.Instance.GetCallableType(this);
			}
		}

		public IType Type
		{
			get
			{
				return CallableType;
			}
		}
		
		public virtual IParameter[] GetParameters()
		{
            if (null != _parameters) return _parameters;
            return _parameters = _provider.Map(_memberInfo.GetParameters());
		}

		public virtual IType ReturnType
		{
			get
			{
				MethodInfo mi = _memberInfo as MethodInfo;
				if (null != mi) return _provider.Map(mi.ReturnType);
				return null;
			}
		}

		public MethodBase MethodInfo
		{
			get { return _memberInfo; }
		}
		
		override public bool Equals(object other)
		{
			if (null == other) return false;
			if (this == other) return true;

			ExternalMethod method = other as ExternalMethod;
			return Equals(method);
		}

		public bool Equals(ExternalMethod other)
		{
			if (null == other) return false;
			if (this == other) return true;

			return _memberInfo.MethodHandle.Value == other._memberInfo.MethodHandle.Value;
		}

		override public int GetHashCode()
		{
			return _memberInfo.MethodHandle.Value.GetHashCode();
		}
		
		override public string ToString()
		{
			return TypeSystemServices.GetSignature(this);
		}
		
		ExternalGenericMethodInfo _genericMethodDefinitionInfo = null;		
		public IGenericMethodInfo GenericInfo
		{
			get
			{
				if (MethodInfo.IsGenericMethodDefinition)
				{
					if (_genericMethodDefinitionInfo == null)
					{
						_genericMethodDefinitionInfo = 
							new ExternalGenericMethodInfo(_provider, this);
					}
					return _genericMethodDefinitionInfo;
				}
				return null;
			}
		}

		ExternalConstructedMethodInfo _genericMethodInfo = null;
		public virtual IConstructedMethodInfo ConstructedInfo
		{
			get
			{
				if (MethodInfo.IsGenericMethod)
				{
					if (_genericMethodInfo == null)
					{
						_genericMethodInfo = new ExternalConstructedMethodInfo(_provider, this);
					}
					return _genericMethodInfo;
				}
				return null;
			}
		}

		protected override Type MemberType
		{
			get
			{
				MethodInfo mi = _memberInfo as MethodInfo;
				if (null != mi) return mi.ReturnType;
				return null;
			}
		}
	}
}
