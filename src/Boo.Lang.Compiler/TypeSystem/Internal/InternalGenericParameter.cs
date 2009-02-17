#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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

using System;
using Boo.Lang.Compiler.Ast;
using System.Collections.Generic;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.TypeSystem
{
    /// <summary>
    /// A generic type parameter of an internal generic type or method.
    /// </summary>
	public class InternalGenericParameter : AbstractGenericParameter, IInternalEntity
	{
		int _position = -1;
		GenericParameterDeclaration _declaration;
		IType[] _baseTypes = null;
		
		public InternalGenericParameter(TypeSystemServices tss, GenericParameterDeclaration declaration) : base(tss)
		{
			_declaration = declaration;
		}

		public InternalGenericParameter(TypeSystemServices tss, GenericParameterDeclaration declaration, int position)
				: this(tss, declaration)
		{
			_position = position;
		}

		public override int GenericParameterPosition
		{
			get 
			{
				if (_position == -1)
				{
					IGenericParameter[] parameters = 
						DeclaringMethod != null ? DeclaringMethod.GenericInfo.GenericParameters : DeclaringType.GenericInfo.GenericParameters;
					
					_position = Array.IndexOf(parameters, this);
				}

				return _position;
			}
		}

		public override IType[] GetTypeConstraints()
		{
			if (_baseTypes == null)
			{
				List<IType> baseTypes = new List<IType>();
				foreach (TypeReference baseTypeReference in _declaration.BaseTypes)
				{
					IType baseType = (IType)baseTypeReference.Entity;
					if (baseType != null)
					{
						baseTypes.Add(baseType);
					}
					else if (IsDeclaringTypeReference(baseTypeReference))
					{
						baseTypes.Add(DeclaringType);
					}
				}

				_baseTypes = baseTypes.ToArray();
			}
			
			return _baseTypes;			
		}

		public override IEntity DeclaringEntity
		{
			get { return TypeSystemServices.GetEntity(_declaration.ParentNode); }
		}
		
		public override string Name
		{
			get { return _declaration.Name; }
		}

		public Node Node
		{
			get { return _declaration; }
		}

		override public bool IsValueType
		{
			get { return HasConstraint(GenericParameterConstraints.ValueType); }
		}

		override public bool IsClass
		{
			get { return HasConstraint(GenericParameterConstraints.ReferenceType); }
		}

		override public bool MustHaveDefaultConstructor
		{
			get { return HasConstraint(GenericParameterConstraints.Constructable); }
		}

		override public Variance Variance
		{
			get
			{
				if (HasConstraint(GenericParameterConstraints.Covariant))
				{
					return Variance.Covariant;
				}
				else if (HasConstraint(GenericParameterConstraints.Contravariant))
				{
					return Variance.Contravariant;
				}
				return Variance.Invariant;
			}
		}

		bool HasConstraint(GenericParameterConstraints flag)
		{
			return (_declaration.Constraints & flag) == flag;
		}

		private bool IsDeclaringTypeReference(TypeReference reference)
		{
			if (!(reference is GenericTypeReference && DeclaringType is InternalClass))
				return false;
			return Node.ParentNode == ((InternalClass)DeclaringType).Node;
		}
	}
}
