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

using System;
using Boo.Lang;
using Boo.Lang.Ast;
using System.Reflection;

namespace Boo.Lang.Compiler.Bindings
{
	public class InternalTypeBinding : AbstractInternalTypeBinding
	{		
		IConstructorBinding[] _constructors;
		
		ITypeBinding _baseType;
		
		internal InternalTypeBinding(BindingManager manager, TypeDefinition typeDefinition) :
			base(manager, typeDefinition)
		{
		}		
		
		public override ITypeBinding BaseType
		{
			get
			{
				if (null == _baseType)
				{
					foreach (TypeReference baseType in _typeDefinition.BaseTypes)
					{
						ITypeBinding binding = _bindingManager.GetBoundType(baseType);
						if (binding.IsClass)
						{
							_baseType = binding;
							break;
						}
					}
				}
				return _baseType;
			}
		}
		
		public override bool IsSubclassOf(ITypeBinding type)
		{			
			foreach (TypeReference baseTypeReference in _typeDefinition.BaseTypes)
			{
				ITypeBinding baseType = _bindingManager.GetBoundType(baseTypeReference);
				if (type == baseType || baseType.IsSubclassOf(type))
				{
					return true;
				}
			}
			return false;
		}
		
		public override IConstructorBinding[] GetConstructors()
		{
			if (null == _constructors)
			{
				List constructors = new List();
				foreach (TypeMember member in _typeDefinition.Members)
				{					
					if (member.NodeType == NodeType.Constructor)
					{
						constructors.Add(BindingManager.GetBinding(member));
					}
				}
				_constructors = (IConstructorBinding[])constructors.ToArray(typeof(IConstructorBinding));
			}
			return _constructors;
		}
		
		public override string ToString()
		{
			return string.Format("InternalTypeBinding<TypeDefinition={0}>", _typeDefinition);
		}
	}
}
