#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
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

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Reflection;
	
	public class ExternalMethod : IMethod
	{
		TypeSystemServices _typeSystemServices;
		
		MethodBase _mi;
		
		IParameter[] _parameters;
		
		ICallableType _type;
		
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
