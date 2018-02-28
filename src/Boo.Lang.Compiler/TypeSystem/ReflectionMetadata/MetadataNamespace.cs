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
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem.ReflectionMetadata
{
	using System.Linq;
	using System.Reflection.Metadata;
	using TypeAttributes = System.Reflection.TypeAttributes;
	using Boo.Lang.Environments;

	internal class MetadataNamespace : AbstractNamespace
	{
		private readonly MemoizedFunction<string, MetadataNamespace> _childNamespaces;
		private readonly MemoizedFunction<string, List<TypeDefinition>> _typeLists;
		private List<INamespace> _modules;
		private readonly MetadataTypeSystemProvider _provider;
		private readonly MetadataReader _reader;

		private readonly Dictionary<string, List<IEntity>> _cache = new Dictionary<string, List<IEntity>>();

		public MetadataNamespace(MetadataTypeSystemProvider provider, MetadataReader reader)
		{
			_childNamespaces = new MemoizedFunction<string, MetadataNamespace>(StringComparer.Ordinal, CreateChildNamespace);
			_typeLists = new MemoizedFunction<string, List<TypeDefinition>>(StringComparer.Ordinal, NewTypeList);
			_provider = provider;
			_reader = reader;
		}

		public MetadataNamespace Produce(string name)
		{
			return _childNamespaces.Invoke(name);
		}

		private MetadataNamespace CreateChildNamespace(string name)
		{
			return new ChildMetadataNamespace(this, name, _reader);
		}

		private sealed class ChildMetadataNamespace : MetadataNamespace
		{
			private readonly MetadataNamespace _parent;
			private string _name;

			public ChildMetadataNamespace(MetadataNamespace parent, string name, MetadataReader reader)
				: base(parent._provider, reader)
			{
				_parent = parent;
				_name = name;
			}

			public override string Name
			{
				get { return _name; }
			}

			public override INamespace ParentNamespace
			{
				get { return _parent; }
			}

			public override string ToString()
			{
				return FullName;
			}
		}

		#region Implementation of INamespace

		private bool DoResolve(string name, out List<IEntity> resultingSet)
		{
			var typesToConsider = EntityType.Any;
			resultingSet = new List<IEntity>();
			try
			{
				if (ResolveChildNamespace(resultingSet, name, typesToConsider))
					return true;
				if (ResolveType(resultingSet, name, typesToConsider))
					return true;
				if (ResolveModules(resultingSet, name, typesToConsider))
					return true;
				resultingSet = null;
				return false;
			}
			finally
			{
				_cache.Add(name, resultingSet);
			}
		}

		private bool CachedResolve(string name, EntityType typesToConsider, ICollection<IEntity> resultingSet)
		{
			List<IEntity> list;
			var result = _cache.TryGetValue(name, out list) || DoResolve(name, out list);
			if (result)
			{
				if (list == null)
					return false;

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

		public override bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			/*
			if (ResolveChildNamespace(resultingSet, name, typesToConsider))
				return true;
			if (ResolveType(resultingSet, name, typesToConsider))
				return true;
			return ResolveModules(resultingSet, name, typesToConsider);
			*/
			return CachedResolve(name, typesToConsider, resultingSet);
		}

		private bool ResolveModules(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			if (_modules == null)
				return false;

			bool found = false;
			foreach (INamespace module in _modules)
				if (module.Resolve(resultingSet, name, typesToConsider))
					found = true;
			return found;
		}

		private bool ResolveType(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			List<TypeDefinition> types;
			if (Entities.IsFlagSet(typesToConsider, TypeSystem.EntityType.Type)
				&& _typeLists.TryGetValue(name, out types))
			{
				foreach (IEntity entity in EntitiesFor(types))
					resultingSet.Add(entity);
				return true;
			}
			return false;
		}

		private bool ResolveChildNamespace(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			MetadataNamespace childNamespace;
			if (Entities.IsFlagSet(typesToConsider, TypeSystem.EntityType.Namespace)
				&& _childNamespaces.TryGetValue(name, out childNamespace))
			{
				resultingSet.Add(childNamespace);
				return true;
			}
			return false;
		}

		private IEnumerable<IEntity> EntitiesFor(List<TypeDefinition> types)
		{
			foreach (var type in types)
				yield return _provider.Map(type, _reader);
		}

		public override IEnumerable<IEntity> GetMembers()
		{
			foreach (MetadataNamespace child in _childNamespaces.Values)
				yield return child;

			foreach (var typeList in _typeLists.Values)
				foreach (var type in typeList)
					yield return _provider.Map(type, _reader);

			if (null != _modules)
				foreach (INamespace @namespace in _modules)
					foreach (IEntity member in @namespace.GetMembers())
						yield return member;
		}

		#endregion

		public void Add(TypeDefinition type)
		{
			string typeName = TypeUtilities.RemoveGenericSuffixFrom(_reader.GetString(type.Name));
			TypeListFor(typeName).Add(type);
			if (IsModule(type))
				AddModule(type);
		}

		private void AddModule(TypeDefinition type)
		{
			if (_modules == null)
				_modules = new List<INamespace>();
			_modules.Add(_provider.Map(type, _reader));
		}

		private List<TypeDefinition> TypeListFor(string name)
		{
			return _typeLists.Invoke(name);
		}

		private bool IsModule(TypeDefinition type)
		{
			var sealedClass = TypeAttributes.Class & TypeAttributes.Sealed;
			var result = (type.Attributes & sealedClass) == sealedClass;
			result &= (type.Attributes & TypeAttributes.NestedPublic) == 0;
			return result && HasModuleMarkerAttribute(type);
		}

		private bool HasModuleMarkerAttribute(TypeDefinition type)
		{
			var attrs = _provider.GetCustomAttributeTypes(type.GetCustomAttributes(), _reader);
			var tss = My<TypeSystemServices>.Instance;
			var module = tss.Map(Types.ModuleAttribute);
			var extension = tss.Map(Types.ClrExtensionAttribute);
			return attrs.Any(a => a == module || a == extension);
		}

		private static List<TypeDefinition> NewTypeList(string name)
		{
			return new List<TypeDefinition>();
		}
	}
}
