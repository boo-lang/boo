#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
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

namespace Boo.Lang.Compiler.Infos
{
	using System;
	using System.Reflection;
	using Boo.Lang.Compiler.Services;
	
	public class ExternalMethodInfo : IMethodInfo
	{
		DefaultInfoService _bindingService;
		
		MethodBase _mi;
		
		internal ExternalMethodInfo(DefaultInfoService manager, MethodBase mi)
		{
			_bindingService = manager;
			_mi = mi;
		}
		
		public ITypeInfo DeclaringType
		{
			get
			{
				return _bindingService.AsTypeInfo(_mi.DeclaringType);
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
		
		public virtual InfoType InfoType
		{
			get
			{
				return InfoType.Method;
			}
		}
		
		public ITypeInfo BoundType
		{
			get
			{
				return ReturnType;
			}
		}
		
		public int ParameterCount
		{
			get
			{
				return _mi.GetParameters().Length;
			}
		}
		
		public ITypeInfo GetParameterType(int parameterIndex)
		{
			return _bindingService.AsTypeInfo(_mi.GetParameters()[parameterIndex].ParameterType);
		}
		
		public ITypeInfo ReturnType
		{
			get
			{
				MethodInfo mi = _mi as MethodInfo;
				if (null != mi)
				{
					return _bindingService.AsTypeInfo(mi.ReturnType);
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
			return DefaultInfoService.GetSignature(this);
		}
	}
	
	public class ExternalConstructorInfo : ExternalMethodInfo, IConstructorInfo
	{
		public ExternalConstructorInfo(DefaultInfoService manager, ConstructorInfo ci) : base(manager, ci)
		{			
		}
		
		override public InfoType InfoType
		{
			get
			{
				return InfoType.Constructor;
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
