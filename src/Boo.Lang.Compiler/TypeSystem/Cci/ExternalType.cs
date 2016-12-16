#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//	   * Redistributions of source code must retain the above copyright notice,
//	   this list of conditions and the following disclaimer.
//	   * Redistributions in binary form must reproduce the above copyright notice,
//	   this list of conditions and the following disclaimer in the documentation
//	   and/or other materials provided with the distribution.
//	   * Neither the name of Rodrigo B. de Oliveira nor the names of its
//	   contributors may be used to endorse or promote products derived from this
//	   software without specific prior written permission.
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
using System.Reflection;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.Util;
using Microsoft.Cci;
using Microsoft.Cci.Immutable;

namespace Boo.Lang.Compiler.TypeSystem.Cci
{
	public class ExternalType : IType
	{
        protected ICciTypeSystemProvider _provider;

        private readonly ITypeDefinition _type;

        private IType[] _interfaces;

        private IEntity[] _members;

		private Dictionary<string, List<IEntity>> _cache;

        private int _typeDepth = -1;

        private string _primitiveName;

        private string _fullName;

		private string _name;

        public ExternalType(ICciTypeSystemProvider tss, INamedTypeDefinition type)
		{
			if (null == type) throw new ArgumentException("type");
			_provider = tss;
			_type = type;
		}

		public virtual string FullName
		{
			get
			{
				if (null != _fullName) return _fullName;
				return _fullName = BuildFullName();
			}
		}

		internal string PrimitiveName
		{
			get { return _primitiveName; }

			set { _primitiveName = value; }
		}

		public virtual string Name
		{
			get
			{
				if (null != _name) return _name;
                return _name = TypeUtilities.RemoveGenericSuffixFrom(TypeHelper.GetTypeName(_type));
			}
		}

		public EntityType EntityType
		{
			get { return EntityType.Type; }
		}

		public IType Type
		{
			get { return this; }
		}

		public virtual bool IsFinal
		{
			get { return _type.IsSealed; }
		}

		public bool IsByRef
		{
			get { return false; } //ByRef types are not supported in CCI; ByRef params are
		}

		public virtual IEntity DeclaringEntity
		{
			get { return DeclaringType;  }
		}

		public IType DeclaringType
		{
			get
			{
			    var nested = _type as INestedTypeDefinition;
				return nested != null
				       	? _provider.Map(nested.ContainingTypeDefinition)
				       	: null;
			}
		}

		public bool IsDefined(IType attributeType)
		{
			var type = attributeType as ExternalType;
			if (null == type) return false;
		    return MetadataUtil.IsAttributeDefined(_type, type.ActualType);
		}

		public virtual IType ElementType
		{
		    get
		    {
		        var at = _type as Microsoft.Cci.IArrayType;
		        return _provider.Map(at != null ? at.ElementType.ResolvedType : _type);
		    }
		}

		public virtual bool IsClass
		{
			get { return _type.IsClass; }
		}

		public bool IsAbstract
		{
			get { return _type.IsAbstract; }
		}

		public bool IsInterface
		{
			get { return _type.IsInterface; }
		}

		public bool IsEnum
		{
			get { return _type.IsEnum; }
		}

		public virtual bool IsValueType
		{
			get { return _type.IsValueType; }
		}

		public bool IsArray
		{
			get { return false; }
		}

		public bool IsPointer
		{
            get { return _type is IPointerType; }
		}
		
		public virtual bool IsVoid
		{
			get { return false; }
		}

		public virtual IType BaseType
		{
			get
			{
				var baseType = _type.BaseClasses.FirstOrDefault();
				return null == baseType
				       	? null
				       	: _provider.Map(baseType.ResolvedType);
			}
		}

	    private ITypeDefinitionMember _defaultMember;

	    private bool _defaultMemberFound;

	    private static readonly ITypeReference _defaultMemberAttribute = SystemTypeMapper.GetTypeReference(typeof(DefaultMemberAttribute));

        protected virtual ITypeDefinitionMember[] GetDefaultMembers()
        {
            if (!_defaultMemberFound)
            {
                var currentType = ActualType;
                while (_defaultMember == null && currentType != null)
                {
                    _defaultMember = currentType.GetMatchingMembers(
                            t => MetadataUtil.IsAttributeDefined(t, _defaultMemberAttribute))
                        .FirstOrDefault();
                    if (_defaultMember == null)
                        currentType = (INamedTypeDefinition) currentType.BaseClasses.FirstOrDefault();
                }
                _defaultMemberFound = true;
            }
            if (_defaultMember != null)
                return new []{_defaultMember};
            return new ITypeDefinitionMember[0];
        }

		public IEntity GetDefaultMember()
		{
			return _provider.Map(GetDefaultMembers());
		}

		public ITypeDefinition ActualType
		{
			get { return _type; }
		}

		public virtual bool IsSubclassOf(IType other)
		{
			var external = other as ExternalType;
			if (external == null)
				return false;

            return TypeHelper.Type1DerivesFromType2(_type, external._type)
                || (external.IsInterface && TypeHelper.Type1ImplementsType2(_type, external._type));
		}

		public virtual bool IsAssignableFrom(IType other)
		{
			var external = other as ExternalType;
			if (null == external)
			{
				if (EntityType.Null == other.EntityType)
				{
					return !IsValueType;
				}
				return other.IsSubclassOf(this);
			}
			if (other == _provider.Map(Types.Void))
			{
				return false;
			}
		    return TypeHelper.Type1DerivesFromOrIsTheSameAsType2(_type, external._type) ||
		        TypeHelper.Type1ImplementsType2(_type, external._type);
		}

		public virtual IType[] GetInterfaces()
		{
			if (_interfaces == null)
			{
				_interfaces = _type.Interfaces.Select(i => _provider.Map(i.ResolvedType)).ToArray();
			}
			return _interfaces;
		}

		private void BuildCache()
		{
			_cache = new Dictionary<string, List<IEntity>>();
			foreach (var member in _members)
			{
				List<IEntity> list;
				if (!_cache.TryGetValue(member.Name, out list))
				{
					list = new List<IEntity>();
					_cache.Add(member.Name, list);
				}
				list.Add(member);
			}
		}

		public virtual IEnumerable<IEntity> GetMembers()
		{
			if (null == _members)
			{
				IEntity[] members = CreateMembers();
				_members = members;
				BuildCache();
			}
			return _members;
		}

		protected virtual IEntity[] CreateMembers()
		{
		    return DeclaredMembers().Select(dm => _provider.Map(dm)).ToArray();
		}

        private IEnumerable<ITypeDefinitionMember> DeclaredMembers()
		{
			return _type.Members;
		}

		public int GetTypeDepth()
		{
			if (-1 == _typeDepth)
			{
				_typeDepth = GetTypeDepth(_type);
			}
			return _typeDepth;
		}

		public virtual INamespace ParentNamespace
		{
			get { return null; }
		}

		private bool CachedResolve(string name, EntityType typesToConsider, ICollection<IEntity> resultingSet) 
		{
			if (_cache == null)
			{
				GetMembers();
			}
			List<IEntity> list;
			var result = _cache.TryGetValue(name, out list);
			if (result)
			{
				result = false;
				foreach (var entity in list)
				{
					if (Entities.IsFlagSet(typesToConsider, entity.EntityType))
					{
						result = true;
						resultingSet.Add(entity);
					}
				}
			}
			return result;
		}

		public virtual bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			bool found = CachedResolve(name, typesToConsider, resultingSet);
			
			//bool found = My<NameResolutionService>.Instance.Resolve(name, GetMembers(), typesToConsider, resultingSet);

			if (IsInterface)
			{
				if (_provider.Map(typeof(object)).Resolve(resultingSet, name, typesToConsider))
					found = true;
				foreach (IType baseInterface in GetInterfaces())
					found |= baseInterface.Resolve(resultingSet, name, typesToConsider);
			}
			else
			{
				if (!found || TypeSystemServices.ContainsMethodsOnly(resultingSet))
				{
					IType baseType = BaseType;
					if (null != baseType)
						found |= baseType.Resolve(resultingSet, name, typesToConsider);
				}
			}
			return found;
		}

        public override string ToString()
		{
			return this.DisplayName();
		}

		private static int GetTypeDepth(ITypeDefinition type)
		{
			if (type.IsInterface)
			{
				return GetInterfaceDepth(type);
			}
			return GetClassDepth(type);
		}

        private static int GetClassDepth(ITypeDefinition type)
		{
			var depth = 0;
		    var objectType = CompilerContext.Current.Host.PlatformType.SystemObject.ResolvedType;
			while (type != null && !TypeHelper.TypesAreEquivalent(type, objectType))
			{
				var baseType = type.BaseClasses.FirstOrDefault();
			    type = baseType != null ? baseType.ResolvedType : null;
				++depth;
			}
			return depth;
		}

        private static int GetInterfaceDepth(ITypeDefinition type)
		{
			var interfaces = type.Interfaces;
			var current = interfaces.Select(i => GetInterfaceDepth(i.ResolvedType)).Concat(new[] {0}).Max();
		    return current + 1;
		}

		protected virtual string BuildFullName()
		{
            return _primitiveName ?? TypeUtilitiesCci.GetFullName((INamedTypeDefinition)_type);
		}

		ExternalGenericTypeInfo _genericTypeDefinitionInfo;
		public virtual IGenericTypeInfo GenericInfo
		{
			get
			{
				if (ActualType.IsGeneric)
					return _genericTypeDefinitionInfo ?? (_genericTypeDefinitionInfo = new ExternalGenericTypeInfo(_provider, this));
				return null;
			}
		}

		ExternalConstructedTypeInfo _genericTypeInfo;
		public virtual IConstructedTypeInfo ConstructedInfo
		{
			get
			{
                if (ActualType is IGenericTypeInstanceReference)
					return _genericTypeInfo ?? (_genericTypeInfo = new ExternalConstructedTypeInfo(_provider, this));
				return null;
			}
		}

		private ArrayTypeCache _arrayTypes;

		public IArrayType MakeArrayType(int rank)
		{
			if (null == _arrayTypes)
				_arrayTypes = new ArrayTypeCache(this);
			return _arrayTypes.MakeArrayType(rank);
		}

		public IType MakePointerType()
		{
			return _provider.Map(PointerType.GetPointerType(_type, CompilerContext.Current.Host.InternFactory));
		}
	}
}

