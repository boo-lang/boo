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
	using System.Reflection;
	
	public class ExternalMethod : IMethod
	{
		TypeSystemServices _typeSystemServices;
		
		MethodBase _mi;
		
		IParameter[] _parameters;
		
		ICallableType _type;

		int _acceptVarArgs = -1;
		
		internal ExternalMethod(TypeSystemServices manager, MethodBase mi)
		{
			_typeSystemServices = manager;
			_mi = mi;
		}
		
		public IType DeclaringType
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
				return _mi.IsFamily;
			}
		}
		
		public bool IsAbstract
		{
			get
			{
				return _mi.IsAbstract;
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
		
		public string Name
		{
			get
			{
				return _mi.Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _mi.DeclaringType.FullName + "." + _mi.Name;
			}
		}

		public bool AcceptVarArgs
		{
			get
			{
				if (_acceptVarArgs == -1)
				{
					ParameterInfo[] parameters = _mi.GetParameters();
					if (parameters.Length > 0
						&& System.Attribute.IsDefined(parameters[parameters.Length-1], typeof(System.ParamArrayAttribute)))
					{
						_acceptVarArgs = 1;
					}
					else
					{
						_acceptVarArgs = 0;
					}
				}
				return _acceptVarArgs == 1;
			}
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
				if (null == _type)
				{
					_type = _typeSystemServices.GetCallableType(this);
				}
				return _type;
			}
		}
		
		public IType Type
		{
			get
			{
				return CallableType;
			}
		}
		
		public IParameter[] GetParameters()
		{
			if (null == _parameters)
			{
				_parameters = _typeSystemServices.Map(_mi.GetParameters());
			}
			return _parameters;
		}
		
		public IType ReturnType
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
			get
			{
				return _mi;
			}
		}
		
		override public bool Equals(object other)
		{
			ExternalMethod rhs = other as ExternalMethod;
			if (null == rhs)
			{
				return false;
			}
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
	}
	
	public class ExternalConstructor : ExternalMethod, IConstructor
	{
		public ExternalConstructor(TypeSystemServices manager, ConstructorInfo ci) : base(manager, ci)
		{			
		}
		
		override public EntityType EntityType
		{
			get
			{
				return EntityType.Constructor;
			}
		}
		
		public ConstructorInfo ConstructorInfo
		{
			get
			{
				return (ConstructorInfo)MethodInfo;
			}
		}
	}
}
