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
	
	public class InternalFieldBinding : AbstractInternalBinding, IFieldBinding
	{
		BindingManager _bindingManager;
		Field _field;
		
		public InternalFieldBinding(BindingManager bindingManager, Field field)
		{
			_bindingManager = bindingManager;
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
				return _bindingManager.GetBoundType(_field.Type);
			}
		}
		
		public ITypeBinding DeclaringType
		{
			get
			{
				return (ITypeBinding)BindingManager.GetBinding(_field.ParentNode);
			}
		}
		
		public object StaticValue
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		
		public override Node Node
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
	}
}
