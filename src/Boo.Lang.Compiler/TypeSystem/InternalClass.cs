#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using Boo.Lang;
	using Boo.Lang.Compiler.Ast;
	using System.Reflection;
	
	public class InternalClass : AbstractInternalType
	{		
		IConstructor[] _constructors;
		
		IType _baseType;
		
		int _typeDepth = -1;
		
		internal InternalClass(TypeSystemServices manager, TypeDefinition typeDefinition) :
			base(manager, typeDefinition)
		{
		}

		override public bool IsValueType
		{
			get
			{
				return _typeSystemServices.ValueTypeType == BaseType;
			}
		}
		
		override public IType BaseType
		{
			get
			{
				if (null == _baseType)
				{
					foreach (TypeReference baseType in _typeDefinition.BaseTypes)
					{
						IType tag = TypeSystemServices.GetType(baseType);
						if (!tag.IsInterface)
						{
							_baseType = tag;
							break;
						}
					}
				}
				return _baseType;
			}
		}
		
		override public bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
		{
			bool found = base.Resolve(targetList, name, flags);
			if (!found || ContainsMethodsOnly(targetList))
			{
				if (BaseType.Resolve(targetList, name, flags))
				{
					found = true;
				}
			}
			return found;
		}
		
		override public int GetTypeDepth()
		{
			if (-1 == _typeDepth)
			{
				_typeDepth = 1+BaseType.GetTypeDepth();
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
		
		bool ContainsMethodsOnly(Boo.Lang.List members)
		{
			foreach (IEntity member in members)
			{
				if (EntityType.Method != member.EntityType)
				{
					return false;
				}
			}
			return true;
		}
	}
}
