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
	public class ExternalFieldBinding : IFieldBinding
	{
		BindingService _bindingManager;
		
		System.Reflection.FieldInfo _field;
		
		public ExternalFieldBinding(BindingService bindingManager, System.Reflection.FieldInfo field)
		{
			_bindingManager = bindingManager;
			_field = field;
		}
		
		public ITypeBinding DeclaringType
		{
			get
			{
				return _bindingManager.AsTypeBinding(_field.DeclaringType);
			}
		}
		
		public string Name
		{
			get
			{
				return _field.Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _field.DeclaringType.FullName + "." + _field.Name;
			}
		}
		
		public bool IsPublic
		{
			get
			{
				return _field.IsPublic;
			}
		}
		
		public bool IsStatic
		{
			get
			{
				return _field.IsStatic;
			}
		}
		
		public bool IsLiteral
		{
			get
			{
				return _field.IsLiteral;
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Field;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return _bindingManager.AsTypeBinding(_field.FieldType);
			}
		}
		
		public System.Type Type
		{
			get
			{
				return _field.FieldType;
			}
		}
		
		public object StaticValue
		{
			get
			{
				return _field.GetValue(null);
			}
		}
		
		public System.Reflection.FieldInfo FieldInfo
		{
			get
			{
				return _field;
			}
		}
	}
}
