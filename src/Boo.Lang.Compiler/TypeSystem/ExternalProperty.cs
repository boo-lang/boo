#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.TypeSystem
{	
	public class ExternalProperty : IProperty
	{
		TypeSystemServices _typeSystemServices;
		
		System.Reflection.PropertyInfo _property;
		
		IParameter[] _parameters;
		
		public ExternalProperty(TypeSystemServices tagManager, System.Reflection.PropertyInfo property)
		{
			_typeSystemServices = tagManager;
			_property = property;
		}
		
		public IType DeclaringType
		{
			get
			{
				return _typeSystemServices.Map(_property.DeclaringType);
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
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.Property;
			}
		}
		
		public IType Type
		{
			get
			{
				return _typeSystemServices.Map(_property.PropertyType);
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
				_parameters = _typeSystemServices.Map(_property.GetIndexParameters());
			}
			return _parameters;
		}
		
		public IMethod GetGetMethod()
		{
			System.Reflection.MethodInfo getter = _property.GetGetMethod(true);
			if (null != getter)
			{
				return (IMethod)_typeSystemServices.Map(getter);
			}
			return null;
		}
		
		public IMethod GetSetMethod()
		{
			System.Reflection.MethodInfo setter = _property.GetSetMethod(true);
			if (null != setter)
			{
				return (IMethod)_typeSystemServices.Map(setter);
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
