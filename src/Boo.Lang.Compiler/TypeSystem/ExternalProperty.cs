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

namespace Boo.Lang.Compiler.TypeSystem
{	
	public class ExternalProperty : IProperty
	{
		TagService _tagService;
		
		System.Reflection.PropertyInfo _property;
		
		IParameter[] _parameters;
		
		public ExternalProperty(TagService tagManager, System.Reflection.PropertyInfo property)
		{
			_tagService = tagManager;
			_property = property;
		}
		
		public IType DeclaringType
		{
			get
			{
				return _tagService.Map(_property.DeclaringType);
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
		
		public ElementType ElementType
		{
			get
			{
				return ElementType.Property;
			}
		}
		
		public IType Type
		{
			get
			{
				return _tagService.Map(_property.PropertyType);
			}
		}
		
		public System.Reflection.PropertyInfo PropertyInfo
		{
			get
			{
				return _property;
			}
		}
		
		public IParameter[] GetParameters()
		{
			if (null == _parameters)
			{
				_parameters = _tagService.Map(_property.GetIndexParameters());
			}
			return _parameters;
		}
		
		public IMethod GetGetMethod()
		{
			System.Reflection.MethodInfo getter = _property.GetGetMethod(true);
			if (null != getter)
			{
				return (IMethod)_tagService.Map(getter);
			}
			return null;
		}
		
		public IMethod GetSetMethod()
		{
			System.Reflection.MethodInfo setter = _property.GetSetMethod(true);
			if (null != setter)
			{
				return (IMethod)_tagService.Map(setter);
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
