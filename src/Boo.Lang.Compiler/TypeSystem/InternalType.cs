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
	using System;
	using Boo.Lang;
	using Boo.Lang.Compiler.Ast;
	using System.Reflection;

	public class EnumType : AbstractInternalType
	{
		internal EnumType(TypeSystemServices tagManager, EnumDefinition enumDefinition) :
			base(tagManager, enumDefinition)
		{
		}
		
		override public IType BaseType
		{
			get
			{
				return _typeSystemServices.EnumType;
			}
		}
		
		override public bool IsSubclassOf(IType type)
		{
			return type == _typeSystemServices.EnumType ||
				_typeSystemServices.EnumType.IsSubclassOf(type);
		}
	}
	
	public class InternalType : AbstractInternalType
	{		
		IConstructor[] _constructors;
		
		IType _baseType;
		
		int _typeDepth = -1;
		
		internal InternalType(TypeSystemServices manager, TypeDefinition typeDefinition) :
			base(manager, typeDefinition)
		{
		}		
		
		override public IType BaseType
		{
			get
			{
				if (null == _baseType)
				{
					if (IsClass)
					{
						foreach (TypeReference baseType in _typeDefinition.BaseTypes)
						{
							IType tag = TypeSystemServices.GetType(baseType);
							if (tag.IsClass)
							{
								_baseType = tag;
								break;
							}
						}
					}
					else if (IsInterface)
					{
						_baseType = _typeSystemServices.ObjectType;
					}
				}
				return _baseType;
			}
		}
		
		override public int GetTypeDepth()
		{
			if (-1 == _typeDepth)
			{
				_typeDepth = CalcTypeDepth();
			}
			return _typeDepth;
		}
		
		override public bool IsSubclassOf(IType type)
		{				
			foreach (TypeReference baseTypeReference in _typeDefinition.BaseTypes)
			{
				IType baseType = TypeSystemServices.GetType(baseTypeReference);
				if (type == baseType || baseType.IsSubclassOf(type))
				{
					return true;
				}
			}
			return _typeSystemServices.IsSystemObject(type);
		}
		
		override public IConstructor[] GetConstructors()
		{
			if (null == _constructors)
			{
				List constructors = new List();
				foreach (TypeMember member in _typeDefinition.Members)
				{					
					if (member.NodeType == NodeType.Constructor && !member.IsStatic)
					{						
						constructors.Add(TypeSystemServices.GetEntity(member));
					}
				}
				_constructors = (IConstructor[])constructors.ToArray(typeof(IConstructor));
			}
			return _constructors;
		}
		
		int CalcTypeDepth()
		{
			if (IsInterface)
			{
				return 1+GetMaxBaseInterfaceDepth();
			}
			return 1+BaseType.GetTypeDepth();
		}
		
		int GetMaxBaseInterfaceDepth()
		{
			int current = 0;
			foreach (TypeReference baseType in _typeDefinition.BaseTypes)
			{
				IType tag = TypeSystemServices.GetType(baseType);
				int depth = tag.GetTypeDepth();
				if (depth > current)
				{
					current = depth;
				}
			}
			return current;
		}
	}
}
