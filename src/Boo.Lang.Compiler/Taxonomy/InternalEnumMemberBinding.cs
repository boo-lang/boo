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
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.Services;
	
	public class InternalEnumMemberInfo : AbstractInternalInfo, IFieldInfo
	{
		DefaultInfoService _bindingService;
		
		EnumMember _member;
		
		public InternalEnumMemberInfo(DefaultInfoService bindingManager, EnumMember member)
		{
			_bindingService = bindingManager;
			_member = member;
		}
		
		public string Name
		{
			get
			{
				return _member.Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _member.DeclaringType.FullName + "." + _member.Name;
			}
		}
		
		public bool IsStatic
		{
			get
			{
				return true;
			}
		}
		
		public bool IsPublic
		{
			get
			{
				return true;
			}
		}
		
		public bool IsLiteral
		{
			get
			{
				return true;
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
				return DeclaringType;
			}
		}
		
		public ITypeInfo DeclaringType
		{
			get
			{
				return (ITypeInfo)DefaultInfoService.GetInfo(_member.ParentNode);
			}
		}
		
		public object StaticValue
		{
			get
			{
				return _member.Initializer.Value;
			}
		}
		
		override public Node Node
		{
			get
			{
				return _member;
			}
		}
	}
}
