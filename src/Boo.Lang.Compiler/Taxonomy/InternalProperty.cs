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
	using Boo.Lang.Compiler.Ast;
	
	public class InternalProperty : IInternalElement, IProperty
	{
		TagService _tagService;
		
		Property _property;
		
		IParameter[] _parameters;
		
		public InternalProperty(TagService tagManager, Property property)
		{
			_tagService = tagManager;
			_property = property;
		}
		
		public IType DeclaringType
		{
			get
			{
				return _tagService.AsTypeInfo(_property.DeclaringType);
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
				return _tagService.GetType(_property.Type);
			}
		}
		
		public IType[] GetParameters()
		{
			if (null == _parameters)
			{
				_parameters = _tagService.Map(_property.Parameters);				
			}
			return _parameters;
		}

		public IMethod GetGetMethod()
		{
			if (null != _property.Getter)
			{
				return (IMethod)TagService.GetTag(_property.Getter);
			}
			return null;
		}
		
		public IMethod GetSetMethod()
		{
			if (null != _property.Setter)
			{
				return (IMethod)TagService.GetTag(_property.Setter);
			}
			return null;
		}
		
		public Node Node
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
