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
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.Services;
	
	public class InternalPropertyBinding : AbstractInternalBinding, IPropertyBinding
	{
		DefaultBindingService _bindingService;
		
		Property _property;
		
		ITypeBinding[] _indexParameters;
		
		public InternalPropertyBinding(DefaultBindingService bindingManager, Property property)
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
				return _property.IsStatic;
			}
		}
		
		public bool IsPublic
		{
			get
			{
				return _property.IsPublic;
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
				return _bindingService.GetBoundType(_property.Type);
			}
		}
		
		public ITypeBinding[] GetIndexParameters()
		{
			if (null == _indexParameters)
			{
				ParameterDeclarationCollection parameters = _property.Parameters;
				_indexParameters = new ITypeBinding[parameters.Count];
				for (int i=0; i<_indexParameters.Length; ++i)
				{
					_indexParameters[i] = _bindingService.GetBoundType(parameters[i]);
				}
			}
			return _indexParameters;
		}

		public IMethodBinding GetGetMethod()
		{
			if (null != _property.Getter)
			{
				return (IMethodBinding)DefaultBindingService.GetBinding(_property.Getter);
			}
			return null;
		}
		
		public IMethodBinding GetSetMethod()
		{
			if (null != _property.Setter)
			{
				return (IMethodBinding)DefaultBindingService.GetBinding(_property.Setter);
			}
			return null;
		}
		
		override public Node Node
		{
			get
			{
				return _property;
			}
		}
		
		public Property Property
		{
			get
			{
				return _property;
			}
		}
	}
}
