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
	using Boo.Lang.Compiler.Services;
	
	public class ExternalPropertyInfo : IPropertyInfo
	{
		DefaultInfoService _bindingService;
		
		System.Reflection.PropertyInfo _property;
		
		ITypeInfo[] _indexParameters;
		
		public ExternalPropertyInfo(DefaultInfoService bindingManager, System.Reflection.PropertyInfo property)
		{
			_bindingService = bindingManager;
			_property = property;
		}
		
		public ITypeInfo DeclaringType
		{
			get
			{
				return _bindingService.AsTypeInfo(_property.DeclaringType);
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
		
		public InfoType InfoType
		{
			get
			{
				return InfoType.Property;
			}
		}
		
		public ITypeInfo BoundType
		{
			get
			{
				return _bindingService.AsTypeInfo(_property.PropertyType);
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
		
		public ITypeInfo[] GetIndexParameters()
		{
			if (null == _indexParameters)
			{
				System.Reflection.ParameterInfo[] parameters = _property.GetIndexParameters();
				_indexParameters = new ITypeInfo[parameters.Length];
				for (int i=0; i<_indexParameters.Length; ++i)
				{
					_indexParameters[i] = _bindingService.AsTypeInfo(parameters[i].ParameterType);
				}
			}
			return _indexParameters;
		}
		
		public IMethodInfo GetGetMethod()
		{
			System.Reflection.MethodInfo getter = _property.GetGetMethod(true);
			if (null != getter)
			{
				return (IMethodInfo)_bindingService.AsInfo(getter);
			}
			return null;
		}
		
		public IMethodInfo GetSetMethod()
		{
			System.Reflection.MethodInfo setter = _property.GetSetMethod(true);
			if (null != setter)
			{
				return (IMethodInfo)_bindingService.AsInfo(setter);
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
