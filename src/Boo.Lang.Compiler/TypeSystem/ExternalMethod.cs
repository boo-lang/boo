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

	public class ExternalMethod : IMethod
	{
		protected TypeSystemServices _typeSystemServices;
		
		MethodBase _mi;
		
		IParameter[] _parameters;
		
		ICallableType _type;

		int _acceptVarArgs = -1;

		int _isDuckTyped = -1;

		int _isExtension = -1;
		
		int _isPInvoke = -1;

		private string _name = null;

		private string _fullName = null;
		
		internal ExternalMethod(TypeSystemServices manager, MethodBase mi)
		{
			_typeSystemServices = manager;
			_mi = mi;
		}
		
		public bool IsExtension
		{
			get
			{
				if (-1 == _isExtension)
				{
					_isExtension = IsStatic && MetadataUtil.IsAttributeDefined(_mi,  Types.ExtensionAttribute)
						? 1
						: 0;
				}
				return 1 == _isExtension;
			}
		}

		public bool IsDuckTyped
		{
			get
			{
				if (-1 == _isDuckTyped)
				{
					_isDuckTyped =
						!ReturnType.IsValueType && MetadataUtil.IsAttributeDefined(_mi, Types.DuckTypedAttribute)
						? 1
						: 0;
				}
				return 1 == _isDuckTyped;
			}
		}
		
		public bool IsPInvoke
		{
			get
			{
				if (-1 == _isPInvoke)
				{
					_isPInvoke = IsStatic && MetadataUtil.IsAttributeDefined(_mi,  Types.DllImportAttribute)
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
				return _typeSystemServices.Map(_mi.DeclaringType);
			}
		}
		
		public bool IsStatic
		{
			get
			{
				return _mi.IsStatic;
			}
		}
		
		public bool IsPublic
		{
			get
			{
				return _mi.IsPublic;
			}
		}
		
		public bool IsProtected
		{
			get
			{
				return _mi.IsFamily || _mi.IsFamilyOrAssembly;
			}
		}

		public bool IsPrivate
		{
			get
			{
				return _mi.IsPrivate;
			}
		}
		
		public bool IsAbstract
		{
			get
			{
				return _mi.IsAbstract;
			}
		}

		public bool IsInternal
		{
			get
			{
				return _mi.IsAssembly;
			}
		}
		
		public bool IsVirtual
		{
			get
			{
				return _mi.IsVirtual;
			}
		}
		
		public bool IsSpecialName
		{
			get
			{
				return _mi.IsSpecialName;
			}
		}
		
		public virtual string Name
		{
			get
			{
				if (_name != null) return _name;
				return _name = _mi.Name;
			}
		}
		
		public virtual string FullName
		{
			get
			{
				if (_fullName != null) return _fullName;
				return _fullName = (DeclaringType + "." + _mi.Name);
			}
		}

		public bool AcceptVarArgs
		{
			get
			{
				if (_acceptVarArgs == -1)
				{
					ParameterInfo[] parameters = _mi.GetParameters();

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

		
		public virtual EntityType EntityType
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
            return _parameters = _typeSystemServices.Map(_mi.GetParameters());
		}
		
		public virtual IType ReturnType
		{
			get
			{
				MethodInfo mi = _mi as MethodInfo;
				if (null != mi)
				{
					return _typeSystemServices.Map(mi.ReturnType);
				}
				return null;
			}
		}
		
		public MethodBase MethodInfo
		{
			get { return _mi; }
		}
		
		override public bool Equals(object other)
		{
			ExternalMethod rhs = other as ExternalMethod;
			if (null == rhs) return false;
			return _mi.MethodHandle.Value == rhs._mi.MethodHandle.Value;
		}
		
		override public int GetHashCode()
		{
			return _mi.MethodHandle.Value.GetHashCode();
		}
		
		override public string ToString()
		{
			return _typeSystemServices.GetSignature(this);
		}
		
#if NET_2_0		
		ExternalGenericMethodDefinitionInfo _genericMethodDefinitionInfo = null;		
		public IGenericMethodDefinitionInfo GenericMethodDefinitionInfo
		{
			get
			{
				if (MethodInfo.IsGenericMethodDefinition)
				{
					if (_genericMethodDefinitionInfo == null)
					{
						_genericMethodDefinitionInfo = 
							new ExternalGenericMethodDefinitionInfo(_typeSystemServices, this);
					}
					return _genericMethodDefinitionInfo;
				}
				return null;
			}
		}

		ExternalGenericMethodInfo _genericMethodInfo = null;
		public virtual IGenericMethodInfo GenericMethodInfo
		{
			get
			{
				if (MethodInfo.IsGenericMethod)
				{
					if (_genericMethodInfo == null)
					{
						_genericMethodInfo = new ExternalGenericMethodInfo(_typeSystemServices, this);
					}
					return _genericMethodInfo;
				}
				return null;
			}
		}	
#else
		IGenericMethodDefinitionInfo IMethod.GenericMethodDefinitionInfo
		{
			get { return null; }
		}
		
		IGenericMethodInfo IMethod.GenericMethodInfo
		{
			get { return null; }
		}		
#endif
	}
}
