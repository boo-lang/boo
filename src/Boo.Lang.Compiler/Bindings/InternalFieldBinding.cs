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
	using Boo.Lang.Compiler.Services;
	using Boo.Lang.Compiler.Ast;
	
	public class InternalFieldInfo : AbstractInternalInfo, IFieldInfo
	{
		DefaultInfoService _bindingService;
		Field _field;
		
		public InternalFieldInfo(DefaultInfoService bindingManager, Field field)
		{
			_bindingService = bindingManager;
			_field = field;
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
		
		public bool IsStatic
		{
			get
			{
				return _field.IsStatic;
			}
		}
		
		public bool IsPublic
		{
			get
			{
				return _field.IsPublic;
			}
		}
		
		public InfoType InfoType
		{
			get
			{
				return InfoType.Field;
			}
		}
		
		public ITypeInfo BoundType
		{
			get
			{
				return _bindingService.GetBoundType(_field.Type);
			}
		}
		
		public ITypeInfo DeclaringType
		{
			get
			{
				return (ITypeInfo)DefaultInfoService.GetInfo(_field.ParentNode);
			}
		}
		
		public bool IsLiteral
		{
			get
			{
				return false;
			}
		}
		
		public object StaticValue
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		
		override public Node Node
		{
			get
			{
				return _field;
			}
		}
		
		public Field Field
		{
			get
			{
				return _field;
			}
		}
		
		override public string ToString()
		{
			return FullName;
		}
	}
}
