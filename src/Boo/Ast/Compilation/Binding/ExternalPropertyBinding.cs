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

namespace Boo.Ast.Compilation.Binding
{
	public class ExternalPropertyBinding : IPropertyBinding
	{
		BindingManager _bindingManager;
		
		System.Reflection.PropertyInfo _property;
		
		public ExternalPropertyBinding(BindingManager bindingManager, System.Reflection.PropertyInfo property)
		{
			_bindingManager = bindingManager;
			_property = property;
		}
		
		public ITypeBinding DeclaringType
		{
			get
			{
				return _bindingManager.ToTypeBinding(_property.DeclaringType);
			}
		}
		
		public bool IsStatic
		{
			get
			{
				System.Reflection.MethodInfo mi = _property.GetGetMethod();
				if (null != mi)
				{
					return mi.IsStatic;
				}
				return _property.GetSetMethod().IsStatic;
			}
		}
		
		public string Name
		{
			get
			{
				return _property.Name;
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
				return _bindingManager.ToTypeBinding(_property.PropertyType);
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
	}
}
