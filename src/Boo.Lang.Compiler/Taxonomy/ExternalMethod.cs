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

namespace Boo.Lang.Compiler.Taxonomy
{
	using System;
	using System.Reflection;
	
	public class ExternalMethod : IMethod
	{
		TagService _tagService;
		
		MethodBase _mi;
		
		IParameter[] _parameters;
		
		internal ExternalMethod(TagService manager, MethodBase mi)
		{
			_tagService = manager;
			_mi = mi;
		}
		
		public IType DeclaringType
		{
			get
			{
				return _tagService.Map(_mi.DeclaringType);
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
		
		public virtual ElementType ElementType
		{
			get
			{
				return ElementType.Method;
			}
		}
		
		public IType Type
		{
			get
			{
				return ReturnType;
			}
		}
		
		public IParameter[] GetParameters()
		{
			if (null == _parameters)
			{
				_parameters = _tagService.Map(_mi.GetParameters());
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
					return _tagService.Map(mi.ReturnType);
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
			return ElementUtil.GetSignature(this);
		}
	}
	
	public class ExternalConstructor : ExternalMethod, IConstructor
	{
		public ExternalConstructor(TagService manager, ConstructorInfo ci) : base(manager, ci)
		{			
		}
		
		override public ElementType ElementType
		{
			get
			{
				return ElementType.Constructor;
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
