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

namespace Boo.Lang.Compiler.Bindings
{
	using Boo.Lang.Compiler.Services;
	
	public class ExternalPropertyBinding : IPropertyBinding
	{
		DefaultBindingService _bindingService;
		
		System.Reflection.PropertyInfo _property;
		
		ITypeBinding[] _indexParameters;
		
		public ExternalPropertyBinding(DefaultBindingService bindingManager, System.Reflection.PropertyInfo property)
		{
			_bindingService = bindingManager;
			_property = property;
		}
		
		public ITypeBinding DeclaringType
		{
			get
			{
				return _bindingService.AsTypeBinding(_property.DeclaringType);
			}
		}
		
		public bool IsStatic
		{
			get
			{
				return GetAccessor().IsStatic;
			}
		}
		
		public bool IsPublic
		{
			get
			{
				return GetAccessor().IsPublic;
			}
		}
		
		public string Name
		{
			get
			{
				return _property.Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _property.DeclaringType.FullName + "." + _property.Name;
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Property;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return _bindingService.AsTypeBinding(_property.PropertyType);
			}
		}
		
		public System.Type Type
		{
			get
			{
				return _property.PropertyType;
			}
		}
		
		public System.Reflection.PropertyInfo PropertyInfo
		{
			get
			{
				return _property;
			}
		}
		
		public ITypeBinding[] GetIndexParameters()
		{
			if (null == _indexParameters)
			{
				System.Reflection.ParameterInfo[] parameters = _property.GetIndexParameters();
				_indexParameters = new ITypeBinding[parameters.Length];
				for (int i=0; i<_indexParameters.Length; ++i)
				{
					_indexParameters[i] = _bindingService.AsTypeBinding(parameters[i].ParameterType);
				}
			}
			return _indexParameters;
		}
		
		public IMethodBinding GetGetMethod()
		{
			System.Reflection.MethodInfo getter = _property.GetGetMethod(true);
			if (null != getter)
			{
				return (IMethodBinding)_bindingService.AsBinding(getter);
			}
			return null;
		}
		
		public IMethodBinding GetSetMethod()
		{
			System.Reflection.MethodInfo setter = _property.GetSetMethod(true);
			if (null != setter)
			{
				return (IMethodBinding)_bindingService.AsBinding(setter);
			}
			return null;
		}
		
		System.Reflection.MethodInfo GetAccessor()
		{
			System.Reflection.MethodInfo mi = _property.GetGetMethod(true);
			if (null != mi)
			{
				return mi;
			}
			return _property.GetSetMethod(true)
;		}
	}
}
