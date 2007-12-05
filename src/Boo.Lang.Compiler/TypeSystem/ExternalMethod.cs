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

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Reflection;

	public class ExternalMethod : ExternalEntity<MethodBase>, IMethod
	{
		IParameter[] _parameters;
		
		ICallableType _type;

		// TODO: replace by bool?
		int _acceptVarArgs = -1;

		int _isExtension = -1;
		
		int _isPInvoke = -1;

		private int _isMeta = -1;
		
		internal ExternalMethod(TypeSystemServices manager, MethodBase mi) : base(manager, mi)
		{
		}

		public bool IsMeta
		{
			get
			{
				if (-1 == _isMeta)
				{
					_isMeta = IsStatic && MetadataUtil.IsAttributeDefined(_memberInfo, typeof(Boo.Lang.MetaAttribute))
					          	? 1
					          	: 0;
				}
				return _isMeta == 1;
			}
		}
		
		public bool IsExtension
		{
			get
			{
				if (-1 == _isExtension)
				{
					_isExtension = IsStatic && MetadataUtil.IsAttributeDefined(_memberInfo,  Types.ExtensionAttribute)
						? 1
						: 0;
				}
				return 1 == _isExtension;
			}
		}
		
		public bool IsPInvoke
		{
			get
			{
				if (-1 == _isPInvoke)
				{
					_isPInvoke = IsStatic && MetadataUtil.IsAttributeDefined(_memberInfo,  Types.DllImportAttribute)
						? 1
						: 0;
				}
				return 1 == _isPInvoke;
			}
		}
		
		public virtual IType DeclaringType
		{
			get
			{
				return _typeSystemServices.Map(_memberInfo.DeclaringType);
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
				if (_acceptVarArgs == -1)
				{
					ParameterInfo[] parameters = _memberInfo.GetParameters();

					_acceptVarArgs =
						parameters.Length > 0 && IsParamArray(parameters[parameters.Length-1]) ? 1 : 0;
				}
				return _acceptVarArgs == 1;
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
				if (null != _type) return _type;

                return _type = _typeSystemServices.GetCallableType(this);
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
            return _parameters = _typeSystemServices.Map(_memberInfo.GetParameters());
		}

		public virtual IType ReturnType
		{
			get
			{
				MethodInfo mi = _memberInfo as MethodInfo;
				if (null != mi) return _typeSystemServices.Map(mi.ReturnType);
				return null;
			}
		}

		public MethodBase MethodInfo
		{
			get { return _memberInfo; }
		}
		
		override public bool Equals(object other)
		{
			ExternalMethod rhs = other as ExternalMethod;
			if (null == rhs) return false;
			return _memberInfo.MethodHandle.Value == rhs._memberInfo.MethodHandle.Value;
		}
		
		override public int GetHashCode()
		{
			return _memberInfo.MethodHandle.Value.GetHashCode();
		}
		
		override public string ToString()
		{
			return _typeSystemServices.GetSignature(this);
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
							new ExternalGenericMethodInfo(_typeSystemServices, this);
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
						_genericMethodInfo = new ExternalConstructedMethodInfo(_typeSystemServices, this);
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
