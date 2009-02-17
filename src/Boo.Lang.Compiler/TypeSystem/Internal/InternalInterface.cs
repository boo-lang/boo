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

using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem.Internal
{
	public class InternalInterface : AbstractInternalType
	{		
		int _typeDepth = -1;
		
		internal InternalInterface(InternalTypeSystemProvider provider, TypeDefinition typeDefinition) :
			this(provider, typeDefinition, false)
		{
		}

		internal InternalInterface(InternalTypeSystemProvider provider, TypeDefinition typeDefinition, bool isByRef) :
			base(provider, typeDefinition)
		{
			_isByRef = isByRef;
		}

		override public IType BaseType
		{
			get
			{
				return My<TypeSystemServices>.Instance.ObjectType;
			}
		}
		
		override public bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			bool found = base.Resolve(resultingSet, name, typesToConsider);
			
			foreach (TypeReference baseType in _node.BaseTypes)
			{	
				if (TypeSystemServices.GetType(baseType).Resolve(resultingSet, name, typesToConsider))
				{
					found = true;
				}
			}
			
			if (!found)
			{
				// also look in System.Object
				if (BaseType.Resolve(resultingSet, name, typesToConsider))
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
				_typeDepth = 1+GetMaxBaseInterfaceDepth();
			}
			return _typeDepth;
		}
		
		override public bool IsSubclassOf(IType type)
		{				
			foreach (TypeReference baseTypeReference in _node.BaseTypes)
			{
				IType baseType = TypeSystemServices.GetType(baseTypeReference);
				if (type == baseType || baseType.IsSubclassOf(type))
				{
					return true;
				}
			}
			return My<TypeSystemServices>.Instance.IsSystemObject(type);
		}
		
		int GetMaxBaseInterfaceDepth()
		{
			int max = 0;
			foreach (TypeReference baseType in _node.BaseTypes)
			{
				IType tag = TypeSystemServices.GetType(baseType);
				int depth = tag.GetTypeDepth();
				if (depth > max)
				{
					max = depth;
				}
			}
			return max;
		}

		override protected IType CreateElementType()
		{
			return new InternalInterface(_provider, _node, true);
		}
	}
}