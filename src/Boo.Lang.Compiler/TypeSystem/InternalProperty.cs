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
	using Boo.Lang.Compiler.Ast;
	
	public class InternalProperty : IInternalEntity, IProperty
	{
		TypeSystemServices _typeSystemServices;
		
		Property _property;
		
		IParameter[] _parameters;
		
		public InternalProperty(TypeSystemServices tagManager, Property property)
		{
			_typeSystemServices = tagManager;
			_property = property;
		}
		
		public IType DeclaringType
		{
			get
			{
				return (IType)TypeSystemServices.GetEntity(_property.DeclaringType);
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
				return TypeSystemServices.GetType(_property.Type);
			}
		}
		
		public IParameter[] GetParameters()
		{
			if (null == _parameters)
			{
				_parameters = _typeSystemServices.Map(_property.Parameters);				
			}
			return _parameters;
		}

		public IMethod GetGetMethod()
		{
			if (null != _property.Getter)
			{
				return (IMethod)TypeSystemServices.GetEntity(_property.Getter);
			}
			return null;
		}
		
		public IMethod GetSetMethod()
		{
			if (null != _property.Setter)
			{
				return (IMethod)TypeSystemServices.GetEntity(_property.Setter);
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
