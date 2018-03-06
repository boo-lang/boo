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
using System.Reflection.Metadata;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem.ReflectionMetadata
{
	using TypeAttributes = System.Reflection.TypeAttributes;

	public class MetadataExternalType : IType
	{
		protected readonly MetadataTypeSystemProvider _provider;

		protected readonly MetadataReader _reader;

		private readonly TypeDefinition _type;

		private IType[] _interfaces;

		private IEntity[] _members;

		private Dictionary<string, List<IEntity>> _cache;

		private int _typeDepth = -1;

		private string _primitiveName;

		private string _fullName;

		private string _name;

		public MetadataExternalType(MetadataTypeSystemProvider tss, TypeDefinition type, MetadataReader reader)
		{
			_provider = tss;
			_type = type;
			_reader = reader;
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
				return _name = TypeUtilities.RemoveGenericSuffixFrom(_reader.GetString(_type.Name));
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
			get { return (_type.Attributes & TypeAttributes.Sealed) != 0; }
		}

		public virtual bool IsByRef
		{
			get { return false; }
		}

		public virtual IEntity DeclaringEntity
		{
			get { return DeclaringType; }
		}

		public IType DeclaringType
		{
			get
			{
				var declaringType = _type.GetDeclaringType();
				return declaringType.IsNil
						   ? null
						   : _provider.Map(_reader.GetTypeDefinition(declaringType), _reader);
			}
		}

		public bool IsDefined(IType attributeType)
		{
			var coll = _type.GetCustomAttributes();
			if (coll.Count == 0)
				return false;
			var attrs = _provider.GetCustomAttributeTypes(coll, _reader);
			return attrs.Any(a => a.Equals(attributeType));
		}

		public CustomAttributeValue<IType>? GetCustomAttribute(IType attributeType)
		{
			var coll = _type.GetCustomAttributes();
			if (coll.Count == 0)
				return null;
			var attrs = _provider.GetCustomAttributeTypes(coll, _reader).ToArray();
			var idx = Array.IndexOf(attrs, attributeType);
			if (idx == -1)
				return null;
			var value = _reader.GetCustomAttribute(coll.ToArray()[idx]);
			return value.DecodeValue(new MetadataSignatureDecoder(_provider, _reader));
		}

		public virtual IType ElementType
		{
			get { return this; }
		}

		public virtual bool IsClass
		{
			get { return (_type.Attributes & TypeAttributes.Class) != 0; }
		}

		public bool IsAbstract
		{
			get { return (_type.Attributes & TypeAttributes.Abstract) != 0; }
		}

		public bool IsInterface
		{
			get { return (_type.Attributes & TypeAttributes.Interface) != 0; }
		}

		public bool IsEnum
		{
			get { return IsValueType && _provider.GetTypeFromEntityHandle(_type.BaseType, _reader) == My<TypeSystemServices>.Instance.EnumType; }
		}

		public virtual bool IsValueType
		{
			get { return (_type.Attributes & TypeAttributes.Class) == 0; }
		}

		public bool IsArray
		{
			get { return false; }
		}

		public bool IsPointer
		{
			get { return false; }
		}

		public virtual bool IsVoid
		{
			get { return false; }
		}

		public virtual IType BaseType
		{
			get
			{
				var baseType = _type.BaseType;
				return baseType.IsNil ? null : _provider.GetTypeFromEntityHandle(baseType, _reader);
			}
		}

		public IEntity GetDefaultMember()
		{
			throw new NotImplementedException();
		}

		public TypeDefinition ActualType
		{
			get { return _type; }
		}

		public virtual bool IsSubclassOf(IType other)
		{
			if (other.IsInterface)
				return this.Implements(other);
			var otherType = other;
			while (otherType != null)
			{
				if (this == other)
				{
					return true;
				}
				otherType = otherType.BaseType;
			}
			return false;
		}

		private bool Implements(IType intf)
		{
			if (this.GetInterfaces().Contains(intf))
				return true;
			var parent = this.BaseType;
			if (parent == null)
				return false;
			var metParent = parent as MetadataExternalType;
			if (metParent == null)
				return parent.IsSubclassOf(intf);
			else return metParent.Implements(intf);
		}

		public virtual bool IsAssignableFrom(IType other)
		{
			var external = other as MetadataExternalType;
			if (null == external)
			{
				if (other.EntityType == EntityType.Null)
				{
					return !IsValueType;
				}
				if (other.ConstructedInfo != null && this.ConstructedInfo != null
					&& ConstructedInfo.GenericDefinition == other.ConstructedInfo.GenericDefinition)
				{
					for (int i = 0; i < ConstructedInfo.GenericArguments.Length; ++i)
					{
						if (!ConstructedInfo.GenericArguments[i].IsAssignableFrom(other.ConstructedInfo.GenericArguments[i]))
							return false;
					}
					return true;
				}
			}
			return other.IsSubclassOf(this);
		}

		public virtual IType[] GetInterfaces()
		{
			if (_interfaces == null)
			{
				_interfaces = _type.GetInterfaceImplementations()
					.Select(i => _provider.GetTypeFromEntityHandle(_reader.GetInterfaceImplementation(i).Interface, _reader))
					.ToArray();
			}
			return _interfaces;
		}

		private void BuildCache()
		{
			_cache = new Dictionary<string, List<IEntity>>();
		}

		public virtual IEnumerable<IEntity> GetMembers()
		{
			if (_members == null)
			{
				IEntity[] members = CreateMembers();
				_members = members;
				BuildCache();
			}
			return _members;
		}

		protected virtual IEntity[] CreateMembers()
		{
			var result = new List<IEntity>();
			foreach (var field in _type.GetFields())
				result.Add(_provider.Map(this, _reader.GetFieldDefinition(field)));
			foreach (var method in _type.GetMethods())
				result.Add(_provider.Map(this, _reader.GetMethodDefinition(method)));
			foreach (var prop in _type.GetProperties())
				result.Add(_provider.Map(this, _reader.GetPropertyDefinition(prop)));
			foreach (var ev in _type.GetEvents())
				result.Add(_provider.Map(this, _reader.GetEventDefinition(ev)));
			foreach (var nt in _type.GetNestedTypes())
				result.Add(_provider.Map(_reader.GetTypeDefinition(nt), _reader));
			return result.ToArray();
		}

		/*
		private MemberInfo[] DeclaredMembers()
		{
			return _type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
		}
		*/

		public int GetTypeDepth()
		{
			var baseType = this.BaseType;
			return baseType == null ? 1 : baseType.GetTypeDepth() + 1;
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
			if (!_cache.ContainsKey(name))
				LoadCache(name);
			var list = _cache[name];
			if (list != null)
			{
				var result = false;
				foreach (var entity in list)
				{
					if (Entities.IsFlagSet(typesToConsider, entity.EntityType))
					{
						result = true;
						resultingSet.Add(entity);
					}
				}
				return result;
			}
			return false;
		}

		private void LoadCache(string name)
		{
			var matches = My<NameResolutionService>.Instance.EntityNameMatcher;
			var list = new List<IEntity>();
			foreach (var member in _members)
				if (matches(member, name))
					list.Add(member);
			if (list.Count == 0)
				list = null;
			_cache.Add(name, list);
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

		static int GetClassDepth(Type type)
		{
			int depth = 0;
			Type objectType = Types.Object;
			while (type != null && type != objectType)
			{
				type = type.BaseType;
				++depth;
			}
			return depth;
		}

		static int GetInterfaceDepth(Type type)
		{
			Type[] interfaces = type.GetInterfaces();
			if (interfaces.Length > 0)
			{
				int current = 0;
				foreach (Type i in interfaces)
				{
					int depth = GetInterfaceDepth(i);
					if (depth > current)
					{
						current = depth;
					}
				}
				return 1 + current;
			}
			return 1;
		}

		protected virtual string BuildFullName()
		{
			if (_primitiveName != null) return _primitiveName;

			var typename = _reader.GetString(_type.Name);
			var dt = this.DeclaringType;
			if (dt != null)
				return dt.FullName + "." + typename;

			var nameSpace = _reader.GetString(_type.Namespace);
			if (string.IsNullOrEmpty(nameSpace))
				return typename;
			return nameSpace + "." + typename;
		}

		MetadataExternalGenericTypeInfo _genericTypeDefinitionInfo;
		public virtual IGenericTypeInfo GenericInfo
		{
			get
			{
				if (ActualType.GetGenericParameters().Count > 0)
					return _genericTypeDefinitionInfo ?? (_genericTypeDefinitionInfo = new MetadataExternalGenericTypeInfo(_provider, this, _reader));
				return null;
			}
		}

		public virtual IConstructedTypeInfo ConstructedInfo
		{
			get
			{
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
			return new MetadataPointerType(_provider, this, _reader);
		}

		public bool IsGenericType
		{
			get { return this.GenericDefinition != null; }
		}

		public IType GenericDefinition
		{
			get { return IsGenericType ? this : null; }
		}
	}
}

