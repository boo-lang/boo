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
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

using Boo.Lang.Compiler.TypeSystem.Core;

namespace Boo.Lang.Compiler.TypeSystem.ReflectionMetadata
{
	using GenericParameterAttributes = System.Reflection.GenericParameterAttributes;

	public class MetadataExternalGenericParameter : IGenericParameter
	{
		private readonly IMethod _declaringMethod;
		private readonly IType _declaringType;
		private readonly GenericParameter _gp;
		private readonly MetadataReader _reader;
		private readonly MetadataTypeSystemProvider _provider;

		public MetadataExternalGenericParameter(MetadataTypeSystemProvider provider, GenericParameter type, MetadataExternalType parent, MetadataReader reader)
		{
			_declaringType = parent;
			_provider = provider;
			_gp = type;
			_reader = reader;
		}

		public MetadataExternalGenericParameter(MetadataTypeSystemProvider provider, GenericParameter type, MetadataExternalMethod parent, MetadataReader reader)
		{
			_declaringMethod = parent;
			_provider = provider;
			_gp = type;
			_reader = reader;
		}

		public int GenericParameterPosition
		{
			get { return _gp.Index; }
		}

		public string FullName
		{
			get { return string.Format("{0}.{1}", DeclaringEntity.FullName, Name); }
		}

		public IEntity DeclaringEntity
		{
			get
			{
				//NB: do not use ?? op to workaround csc bug generating invalid IL
				return (_declaringMethod != null) ? (IEntity)_declaringMethod : _declaringType;
			}
		}

		public Variance Variance
		{
			get
			{
				GenericParameterAttributes variance = _gp.Attributes & GenericParameterAttributes.VarianceMask;
				switch (variance)
				{
					case GenericParameterAttributes.None:
						return Variance.Invariant;

					case GenericParameterAttributes.Covariant:
						return Variance.Covariant;

					case GenericParameterAttributes.Contravariant:
						return Variance.Contravariant;

					default:
						return Variance.Invariant;
				}
			}
		}

		private IType[] _constraints;

		public IType[] GetTypeConstraints()
		{
			if (_constraints == null)
			{
				_constraints = _gp.GetConstraints()
					.Select(c => _provider.GetTypeFromEntityHandle(_reader.GetGenericParameterConstraint(c).Type, _reader))
					.ToArray();
			}
			return _constraints;
		}

		public bool MustHaveDefaultConstructor
		{
			get
			{
				return (_gp.Attributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint;
			}
		}

		public bool IsClass
		{
			get
			{
				return (_gp.Attributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint;
			}
		}

		public bool IsValueType
		{
			get
			{
				return (_gp.Attributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint;
			}
		}

		public bool IsAbstract
		{ get { return false; } }

		public bool IsInterface
		{ get { return this.GetTypeConstraints().Any(tc => tc.IsInterface); } }

		public bool IsEnum
		{ get { return this.GetTypeConstraints().Any(tc => tc.IsEnum); } }

		public bool IsByRef
		{ get { return false; } }

		public bool IsFinal
		{ get { return false; } }

		public bool IsArray
		{ get { return this.GetTypeConstraints().Any(tc => tc.IsArray); } }

		public bool IsPointer
		{ get { return this.GetTypeConstraints().Any(tc => tc.IsPointer); } }

		public bool IsVoid
		{ get { return false; } }

		public IType ElementType
		{
			get
			{
				if (this.IsArray)
				{
					var arrType = this.GetTypeConstraints().First(tc => tc.IsArray);
					return arrType.ElementType;
				}
				else if (this.IsPointer) {
					var ptrType = this.GetTypeConstraints().First(tc => tc.IsPointer);
					return ptrType.ElementType;
				}
				return null;
			}
		}

		public IType BaseType
		{
			get
			{
				var constraints = this.GetTypeConstraints();
				return constraints.FirstOrDefault(tc => tc.IsClass);
			}
		}

		public IGenericTypeInfo GenericInfo { get { return null; } }

		public IConstructedTypeInfo ConstructedInfo { get { return null; } }

		public IType Type { get { return this; } }

		public INamespace ParentNamespace { get { return null; } }

		public string Name { get { return _reader.GetString(_gp.Name); } }

		public EntityType EntityType
		{
			get { return EntityType.GenericParameter; }
		}

		public override string ToString()
		{
			return Name;
		}

		public int GetTypeDepth()
		{
			throw new NotImplementedException();
		}

		public IEntity GetDefaultMember()
		{
			return null;
		}

		public IType[] GetInterfaces()
		{
			return this.GetTypeConstraints().Where(tc => tc.IsInterface).ToArray();
		}

		public bool IsSubclassOf(IType other)
		{
			return this.GetTypeConstraints().Any(tc => tc.IsSubclassOf(other));
		}

		public bool IsAssignableFrom(IType other)
		{
			return this.GetTypeConstraints().Any(tc => tc.IsAssignableFrom(other));
		}

		public IArrayType MakeArrayType(int rank)
		{
			if (null == _arrayTypes)
				_arrayTypes = new ArrayTypeCache(this);
			return _arrayTypes.MakeArrayType(rank);
		}

		private ArrayTypeCache _arrayTypes;

		public IType MakePointerType()
		{
			throw new NotImplementedException();
		}

		public bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			var result = false;
			foreach (var type in this.GetTypeConstraints())
			{
				result |= type.Resolve(resultingSet, name, typesToConsider);
			}
			return result;
		}

		public IEnumerable<IEntity> GetMembers()
		{
			return Enumerable.Empty<IEntity>();
		}

		public bool IsDefined(IType attributeType)
		{
			return _provider.GetCustomAttributeTypes(_gp.GetCustomAttributes(), _reader).Contains(attributeType);
		}

		public bool IsGenericType
		{
			get { return false; }
		}

		public IType GenericDefinition
		{
			get { return null; }
		}
	}

}
