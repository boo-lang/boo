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
	
	public abstract class AbstractInternalTypeBinding : ITypeBinding, INamespace
	{		
		protected BindingManager _bindingManager;
		
		protected TypeDefinition _typeDefinition;
		
		protected AbstractInternalTypeBinding(BindingManager bindingManager, TypeDefinition typeDefinition)
		{
			_bindingManager = bindingManager;
			_typeDefinition = typeDefinition;
		}
		
		public string FullName
		{
			get
			{
				return _typeDefinition.FullName;
			}
		}
		
		public string Name
		{
			get
			{
				return _typeDefinition.Name;
			}
		}
		
		public virtual IBinding Resolve(string name)
		{			
			foreach (TypeMember member in _typeDefinition.Members)
			{
				if (name == member.Name)
				{					
					IBinding binding = _bindingManager.GetOptionalBinding(member);
					if (null == binding)
					{						
						binding = CreateCorrectBinding(member);
						_bindingManager.Bind(member, binding);
					}	
					
					if (BindingType.Type == binding.BindingType)
					{
						binding = _bindingManager.ToTypeReference((ITypeBinding)binding);
					}
					return binding;
				}
			}
			
			foreach (TypeReference baseType in _typeDefinition.BaseTypes)
			{
				IBinding binding = _bindingManager.GetBoundType(baseType).Resolve(name);
				if (null != binding)
				{
					return binding;
				}
			}
			return null;
		}
		
		IBinding CreateCorrectBinding(TypeMember member)
		{
			switch (member.NodeType)
			{
				case NodeType.Method:
				{
					return new InternalMethodBinding(_bindingManager, (Method)member);
				}
				
				case NodeType.Field:
				{
					return new InternalFieldBinding(_bindingManager, (Field)member);
				}
			}
			throw new NotImplementedException();
		}
		
		public virtual ITypeBinding BaseType
		{
			get
			{
				return null;
			}
		}
		
		public TypeDefinition TypeDefinition
		{
			get
			{
				return _typeDefinition;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return this;
			}
		}
		
		public bool IsClass
		{
			get
			{
				return NodeType.ClassDefinition == _typeDefinition.NodeType;
			}
		}
		
		public bool IsValueType
		{
			get
			{
				return false;
			}
		}
		
		public virtual BindingType BindingType
		{
			get
			{
				return BindingType.Type;
			}
		}
		
		public virtual bool IsSubclassOf(ITypeBinding other)
		{
			return false;
		}
		
		public virtual bool IsAssignableFrom(ITypeBinding other)
		{
			return false;
		}
		
		public virtual IConstructorBinding[] GetConstructors()
		{
			return new IConstructorBinding[0];
		}
	}

}
