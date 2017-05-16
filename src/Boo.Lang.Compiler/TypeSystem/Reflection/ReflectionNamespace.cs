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
using System.Collections;
using System.Collections.Generic;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem.Reflection
{
	internal class ReflectionNamespace : AbstractNamespace
	{
		private readonly MemoizedFunction<string, ReflectionNamespace> _childNamespaces;
		private readonly MemoizedFunction<string, List<Type>> _typeLists;
		private List<INamespace> _modules;
		private readonly IReflectionTypeSystemProvider _provider;

		private readonly Dictionary<string, List<IEntity>> _cache = new Dictionary<string, List<IEntity>>();

		public ReflectionNamespace(IReflectionTypeSystemProvider provider)
		{
			_childNamespaces = new MemoizedFunction<string, ReflectionNamespace>(StringComparer.Ordinal, CreateChildNamespace);
			_typeLists = new MemoizedFunction<string, List<Type>>(StringComparer.Ordinal, NewTypeList);
			_provider = provider;
		}

		public ReflectionNamespace Produce(string name)
		{
			return _childNamespaces.Invoke(name);
		}

		private ReflectionNamespace CreateChildNamespace(string name)
		{
			return new ChildReflectionNamespace(this, name);
		}

		private sealed class ChildReflectionNamespace : ReflectionNamespace
		{
			private readonly ReflectionNamespace _parent;
			private string _name;

			public ChildReflectionNamespace(ReflectionNamespace parent, string name) : base(parent._provider)
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
			List<Type> types;
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
			ReflectionNamespace childNamespace;
			if (Entities.IsFlagSet(typesToConsider, TypeSystem.EntityType.Namespace)
			    && _childNamespaces.TryGetValue(name, out childNamespace))
			{
				resultingSet.Add(childNamespace);
				return true;
			}
			return false;
		}

		private IEnumerable EntitiesFor(List<Type> types)
		{
			foreach (Type type in types)
				yield return _provider.Map(type);
		}

		public override IEnumerable<IEntity> GetMembers()
		{
			foreach (ReflectionNamespace child in _childNamespaces.Values)
				yield return child;

			foreach (List<Type> typeList in _typeLists.Values)
				foreach (Type type in typeList)
					yield return _provider.Map(type);

			if (null != _modules)
				foreach (INamespace @namespace in _modules)
					foreach (IEntity member in @namespace.GetMembers())
						yield return member;
		}

		#endregion

		public void Add(Type type)
		{
			string typeName = TypeUtilities.TypeName(type);
			TypeListFor(typeName).Add(type);
			if (IsModule(type))
				AddModule(type);
		}

		private void AddModule(Type type)
		{
			if (_modules == null)
				_modules = new List<INamespace>();
			_modules.Add(_provider.Map(type));
		}

		private List<Type> TypeListFor(string name)
		{
			return _typeLists.Invoke(name);
		}

		private static bool IsModule(Type type)
		{
			return type.IsClass
				&& type.IsSealed
				&& !type.IsNestedPublic
				&& HasModuleMarkerAttribute(type);
		}

		private static bool HasModuleMarkerAttribute(Type type)
		{
			return MetadataUtil.IsAttributeDefined(type, Types.ModuleAttribute)
				|| MetadataUtil.IsAttributeDefined(type, Types.ClrExtensionAttribute);
		}

		private static List<Type> NewTypeList(string name)
		{
			return new List<Type>();
		}
	}
}
