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
	public class InternalClass : AbstractInternalType
	{
		int _typeDepth = -1;
		bool _isPointer;

		internal InternalClass(InternalTypeSystemProvider provider, TypeDefinition typeDefinition) :
			this(provider, typeDefinition, false)
		{
		}

		internal InternalClass(InternalTypeSystemProvider provider, TypeDefinition typeDefinition, bool isByRef) :
			base(provider, typeDefinition)
		{
			_isByRef = isByRef;
		}

		override public bool IsValueType
		{
			get { return _provider.ValueTypeType == BaseType; }
		}

		override public bool IsPointer
		{
			get { return _isPointer; }
		}

		override public IType BaseType
		{
			get { return FindBaseType(); }
		}

		private IType FindBaseType()
		{
			foreach (TypeReference baseType in _node.BaseTypes)
			{
				IType entity = (IType)baseType.Entity;
				if (null != entity && !entity.IsInterface)
				{
					return entity;
				}
			}
			return null;
		}

		override public bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			bool found = base.Resolve(resultingSet, name, typesToConsider);
			if (null != BaseType)
			{
				found |= BaseType.Resolve(resultingSet, name, typesToConsider);
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
			foreach (TypeReference baseTypeReference in _node.BaseTypes)
			{
				IType baseType = TypeSystemServices.GetType(baseTypeReference);
				if (type == baseType || baseType.IsSubclassOf(type))
				{
					return true;
				}
			}
			return _provider.IsSystemObject(type);
		}
		
		override public IConstructor[] GetConstructors()
		{
			// cache removed because the ast node might be edited (boojay, for instance)
			// later
			// optimize in the future but remember to observe the
			// node for changes
			List constructors = new List();
			foreach (TypeMember member in _node.Members)
				if (member.NodeType == NodeType.Constructor && !member.IsStatic)
					constructors.Add(_provider.EntityFor(member));
			return (IConstructor[])constructors.ToArray(new IConstructor[constructors.Count]);
			
		}

		override protected IType CreateElementType()
		{
			return new InternalClass(_provider, _node, true);
		}

		override public IType MakePointerType()
		{
			InternalClass pt =  new InternalClass(_provider, _node);
			pt._isPointer = true;
			pt._elementType = this;
			return pt;
		}
	}
}

