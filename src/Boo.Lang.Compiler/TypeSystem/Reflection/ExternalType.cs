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
using System.Reflection;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem.Reflection
{
	public class ExternalType : IType
	{
		protected IReflectionTypeSystemProvider _provider;

		private readonly Type _type;

	    private IType[] _interfaces;

	    private IEntity[] _members;

        private Dictionary<string, List<IEntity>> _cache;

        private int _typeDepth = -1;

        private string _primitiveName;

        private string _fullName;

		private string _name;

		public ExternalType(IReflectionTypeSystemProvider tss, Type type)
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
				return _name = TypeUtilities.TypeName(_type);
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
			get { return _type.IsByRef; }
		}

		public virtual IEntity DeclaringEntity
		{
			get { return DeclaringType;  }
		}

		public IType DeclaringType
		{
			get
			{
				var declaringType = _type.DeclaringType;
				return null != declaringType
				       	? _provider.Map(declaringType)
				       	: null;
			}
		}

		public bool IsDefined(IType attributeType)
		{
			var type = attributeType as ExternalType;
            if (type == null) return false;
			return MetadataUtil.IsAttributeDefined(_type, type.ActualType);
		}

		public virtual IType ElementType
		{
			get { return _provider.Map(_type.GetElementType() ?? _type); }
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
			get { return _type.IsPointer; }
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
                return baseType == null ? null : _provider.Map(baseType);
			}
		}
		
		protected virtual MemberInfo[] GetDefaultMembers()
		{
			return ActualType.GetDefaultMembers();
		}

		public IEntity GetDefaultMember()
		{
			return _provider.Map(GetDefaultMembers());
		}

		public Type ActualType
		{
			get { return _type; }
		}

		public virtual bool IsSubclassOf(IType other)
		{
			var external = other as ExternalType;
			if (external == null)
				return false;

			return _type.IsSubclassOf(external._type)
				|| (external.IsInterface && external._type.IsAssignableFrom(_type));
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
				return other.IsSubclassOf(this);
			}
			if (other == _provider.Map(Types.Void))
			{
				return false;
			}
			return _type.IsAssignableFrom(external._type);
		}

		public virtual IType[] GetInterfaces()
		{
			if (null == _interfaces)
			{
				Type[] interfaces = _type.GetInterfaces();
				_interfaces = new IType[interfaces.Length];
				for (int i=0; i<_interfaces.Length; ++i)
				{
					_interfaces[i] = _provider.Map(interfaces[i]);
				}
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
			foreach (var member in DeclaredMembers())
				result.Add(_provider.Map(member));
			return result.ToArray();
		}

		private MemberInfo[] DeclaredMembers()
		{
			return _type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
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

		static int GetTypeDepth(Type type)
		{
			if (type.IsByRef)
			{
				return GetTypeDepth(type.GetElementType());
			}
			if (type.IsInterface)
			{
				return GetInterfaceDepth(type);
			}
			return GetClassDepth(type);
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
				return 1+current;
			}
			return 1;
		}

		protected virtual string BuildFullName()
		{
			if (_primitiveName != null) return _primitiveName;

			// keep builtin names pretty ('ref int' instead of 'ref System.Int32')
			if (_type.IsByRef) return "ref " + ElementType.FullName;

			return TypeUtilities.GetFullName(_type);
		}

		ExternalGenericTypeInfo _genericTypeDefinitionInfo;
		public virtual IGenericTypeInfo GenericInfo
		{
			get
			{
				if (ActualType.IsGenericTypeDefinition)
					return _genericTypeDefinitionInfo ?? (_genericTypeDefinitionInfo = new ExternalGenericTypeInfo(_provider, this));
				return null;
			}
		}

		ExternalConstructedTypeInfo _genericTypeInfo;
		public virtual IConstructedTypeInfo ConstructedInfo
		{
			get
			{
				if (ActualType.IsGenericType && !ActualType.IsGenericTypeDefinition)
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
			return _provider.Map(_type.MakePointerType());
		}
	}
}

