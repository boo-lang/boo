#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.Bindings
{
	using System;
	using Boo.Lang.Ast;
	
	public class InternalEnumMemberBinding : AbstractInternalBinding, IFieldBinding
	{
		BindingManager _bindingManager;
		
		EnumMember _member;
		
		public InternalEnumMemberBinding(BindingManager bindingManager, EnumMember member)
		{
			_bindingManager = bindingManager;
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
				return DeclaringType;
			}
		}
		
		public ITypeBinding DeclaringType
		{
			get
			{
				return (ITypeBinding)BindingManager.GetBinding(_member.ParentNode);
			}
		}
		
		public object StaticValue
		{
			get
			{
				return _member.Initializer.Value;
			}
		}
		
		public override Node Node
		{
			get
			{
				return _member;
			}
		}
	}
}
